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
    public class CommentController : ControllerBase
    {
        private readonly IRepository<Comment> repository;
        private readonly IPublishEndpoint publishEndpoint;

        public CommentController(IRepository<Comment> repository, IPublishEndpoint publishEndpoint)
        {
            this.repository = repository;
            this.publishEndpoint = publishEndpoint;
        }

        [Route("GetAllComments")]
        [HttpGet]
        public async Task<IActionResult> GetAllCommentsAsync([FromQuery]CommentParameters commentParameters) => 
            Ok(await repository.GetAllAsync(commentParameters));

        [Route("GetCommentsByDiscussionId/{discussionId}")]
        public async Task<IActionResult> GetCommentsByDiscussionIdAsync(Guid discussionId)
        {
            var comments = await repository.GetByDiscussionIdAsync(discussionId);
            if (comments is null)
            {
                return BadRequest();
            }
            return Ok(comments);
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
                Content = model.Content, CreatedBy = model.CreatedBy, CreatedDate = DateTime.UtcNow, DiscussionId = model.DiscussionId, Id = Guid.NewGuid()
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
    }
}
