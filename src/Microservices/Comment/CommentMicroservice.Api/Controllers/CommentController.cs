using CommentMicroservice.Api.DTOs;
using CommentMicroservice.Api.Models;
using CommentMicroservice.Api.Services;
using CommentMicroservice.Api.Services.Repository;
using MassTransit;
using MessageBus.Messages;
using Microsoft.AspNetCore.Mvc;

namespace CommentMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IRepository<Comment> repository;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IPaginationService paginationService;

        public CommentController(IRepository<Comment> repository, IPublishEndpoint publishEndpoint, IPaginationService paginationService)
        {
            this.repository = repository;
            this.publishEndpoint = publishEndpoint;
            this.paginationService = paginationService;
        }

        [Route("GetAllComments")]
        [HttpGet]
        public async Task<IActionResult> GetAllCommentsAsync([FromQuery]CommentParameters commentParameters) => 
            Ok(await repository.GetAllAsync(commentParameters));

        [Route("DoesNextAllCommentsPageExist")]
        [HttpGet]
        public async Task<IActionResult> DoesNextAllCommentsPageExistAsync([FromQuery] CommentParameters commentParameters)
        {
            bool doesExist = await paginationService.DoesNextCommentsPageExistAsync(commentParameters);
            return Ok(doesExist);
        }

        [Route("GetCommentsByDiscussionId/{discussionId}")]
        public async Task<IActionResult> GetCommentsByDiscussionIdAsync(Guid discussionId, [FromQuery] CommentParameters commentParameters)
        {
            var comments = await repository.GetByDiscussionIdAsync(discussionId, commentParameters);
            return Ok(comments);
        }

        [Route("DoesNextCommentsByDiscussionIdPageExist/{discussionId}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextCommentsByDiscussionIdPageExistAsync(Guid discussionId, [FromQuery] CommentParameters commentParameters)
        {
            bool doesExist =
                await paginationService.DoesNextCommentsByDiscussionIdPageExistAsync(commentParameters, discussionId);
            return Ok(doesExist);
        }

        [Route("UpdateComment")]
        [HttpPut]
        public async Task<IActionResult> UpdateCommentAsync([FromBody] Comment model)
        {
            var comment = await repository.GetByIdAsync(model.Id);
            if (comment is null)
               return BadRequest();

            return Ok(await repository.UpdateAsync(model));
        }

        [Route("DeleteCommentById/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCommentByIdAsync(Guid id)
        {
            var comment = await repository.GetByIdAsync(id);
            if (comment is not null)
            {
                await repository.DeleteByIdAsync(id);

                await publishEndpoint.Publish<ICommentDeleted>(new
                {
                    CommentId = comment.Id,
                    CommentCreatedBy = comment.CreatedBy
                });
                return Ok();
            }

            return BadRequest();
        }

        [Route("CreateComment")]
        [HttpPost]
        public async Task<IActionResult> CreateCommentAsync([FromBody] CreateCommentDto model)
        {
            var createdComment = await repository.CreateAsync(new Comment
            {
                Content = model.Content, CreatedBy = model.CreatedBy, CreatedDate = DateTime.UtcNow, DiscussionId = model.DiscussionId, Id = Guid.NewGuid(),
                RepliedOnCommentContent = model.RepliedOnCommentContent, RepliedOnCommentCreatedBy = model.RepliedOnCommentCreatedBy,
                RepliedOnCommentId = model.RepliedOnCommentId
            });

            await publishEndpoint.Publish<ICommentCreated>(new
            {
                CommentId = createdComment.Id, createdComment.CreatedBy
            });

            return Ok(createdComment);
        }

        [Route("GetCommentsByIds")]
        [HttpGet]
        public async Task<IActionResult> GetCommentsByIdsAsync([FromQuery(Name = "ids[]")] params Guid[] ids)
        {
            var comments = await repository.GetByIds(ids);
            return Ok(comments);
        }

        [Route("GetCommentsByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetCommentsByUserNameAsync(string userName, [FromQuery] CommentParameters commentParameters)
        {
            var comments = await repository.GetByUserNameAsync(userName, commentParameters);
            return Ok(comments);
        }

        [Route("DoesNextCommentsByUserNamePageExist/{userName}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextCommentsByUserNamePageExistAsync(string userName, [FromQuery] CommentParameters commentParameters)
        {
            bool doesExist =
                await paginationService.DoesNextCommentsByUserNamePageExistAsync(userName, commentParameters);
            return Ok(doesExist);
        }
    }
}
