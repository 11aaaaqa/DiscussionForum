using CommentMicroservice.Api.DTOs;
using CommentMicroservice.Api.Models;
using CommentMicroservice.Api.Services.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CommentMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestCommentController : ControllerBase
    {
        private readonly IRepository<SuggestedComment> suggestCommentRepository;
        private readonly IRepository<Comment> commentRepository;

        public SuggestCommentController(IRepository<SuggestedComment> suggestCommentRepository, IRepository<Comment> commentRepository)
        {
            this.suggestCommentRepository = suggestCommentRepository;
            this.commentRepository = commentRepository;
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
            return Ok(suggestedComment);
        }

        [Route("AcceptSuggestedComment/{id}")]
        [HttpDelete]
        public async Task<IActionResult> AcceptSuggestedCommentAsync(Guid id)
        {
            var suggestedComment = await suggestCommentRepository.GetByIdAsync(id);
            await suggestCommentRepository.DeleteByIdAsync(id);
            await commentRepository.CreateAsync(new Comment
            {
                Content = suggestedComment.Content,
                CreatedBy = suggestedComment.CreatedBy,
                Id = suggestedComment.Id,
                CreatedDate = suggestedComment.CreatedDate,
                DiscussionId = suggestedComment.DiscussionId
            });

            //TODO: message bus publishes comment was added message

            return Ok();
        }

        [Route("RejectSuggestedComment/{id}")]
        [HttpDelete]
        public async Task<IActionResult> RejectSuggestedCommentAsync(Guid id)
        {
            await suggestCommentRepository.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
