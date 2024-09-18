using CommentMicroservice.Api.DTOs;
using CommentMicroservice.Api.Models;
using CommentMicroservice.Api.Services.Repository;
using MassTransit;
using MessageBus.Messages;
using Microsoft.AspNetCore.Mvc;

namespace CommentMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestCommentController : ControllerBase
    {
        private readonly IRepository<SuggestedComment> suggestCommentRepository;
        private readonly IRepository<Comment> commentRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public SuggestCommentController(IRepository<SuggestedComment> suggestCommentRepository, IRepository<Comment> commentRepository,
            IPublishEndpoint publishEndpoint)
        {
            this.suggestCommentRepository = suggestCommentRepository;
            this.commentRepository = commentRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [Route("GetAllSuggestedComments")]
        [HttpGet]
        public async Task<IActionResult> GetAllSuggestedCommentsAsync() => Ok(await suggestCommentRepository.GetAllAsync());

        [Route("Suggest")]
        [HttpPost]
        public async Task<IActionResult> SuggestCommentAsync([FromBody] SuggestCommentDto model)
        {
            var suggestedComment = await suggestCommentRepository.CreateAsync(new SuggestedComment
            {
                Content = model.Content, CreatedBy = model.CreatedBy, DiscussionId = model.DiscussionId,
                CreatedDate = DateTime.UtcNow, Id = Guid.NewGuid()
            });

            await publishEndpoint.Publish<IUserSuggestedComment>(new
            {
                SuggestedCommentId = suggestedComment.Id,
                SuggestedBy = suggestedComment.CreatedBy
            });

            return Ok(suggestedComment);
        }

        [Route("AcceptSuggestedComment/{id}")]
        [HttpDelete]
        public async Task<IActionResult> AcceptSuggestedCommentAsync(Guid id)   
        {
            var suggestedComment = await suggestCommentRepository.GetByIdAsync(id);
            if (suggestedComment == null) return BadRequest();

            await suggestCommentRepository.DeleteByIdAsync(id);
            await commentRepository.CreateAsync(new Comment
            {
                Content = suggestedComment.Content,
                CreatedBy = suggestedComment.CreatedBy,
                Id = suggestedComment.Id,
                CreatedDate = suggestedComment.CreatedDate,
                DiscussionId = suggestedComment.DiscussionId
            });

            await publishEndpoint.Publish<ISuggestedCommentAccepted>(new { AcceptedCommentId = suggestedComment.Id, suggestedComment.CreatedBy});

            return Ok();
        }

        [Route("RejectSuggestedComment/{id}")]
        [HttpDelete]
        public async Task<IActionResult> RejectSuggestedCommentAsync(Guid id)
        {
            var suggestedComment = await suggestCommentRepository.GetByIdAsync(id);
            if (suggestedComment is not null)
            {
                await publishEndpoint.Publish<ISuggestedCommentRejected>(new { RejectedCommentId = suggestedComment.Id, suggestedComment.CreatedBy});
                await suggestCommentRepository.DeleteByIdAsync(id);
                return Ok();
            }
            return BadRequest();
        }

        [Route("GetSuggestedCommentsByIds")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedCommentsByIdsAsync([FromQuery(Name = "ids[]")]params Guid[] ids)
        {
            var suggestedComments = await suggestCommentRepository.GetByIds(ids);
            return Ok(suggestedComments);
        }
    }
}
