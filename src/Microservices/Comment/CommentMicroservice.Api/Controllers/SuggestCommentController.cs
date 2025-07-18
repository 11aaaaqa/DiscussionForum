﻿using CommentMicroservice.Api.DTOs;
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
    public class SuggestCommentController : ControllerBase
    {
        private readonly IRepository<SuggestedComment> suggestCommentRepository;
        private readonly IRepository<Comment> commentRepository;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IPaginationService paginationService;

        public SuggestCommentController(IRepository<SuggestedComment> suggestCommentRepository, IRepository<Comment> commentRepository,
            IPublishEndpoint publishEndpoint, IPaginationService paginationService)
        {
            this.suggestCommentRepository = suggestCommentRepository;
            this.commentRepository = commentRepository;
            this.publishEndpoint = publishEndpoint;
            this.paginationService = paginationService;
        }

        [Route("GetAllSuggestedComments")]
        [HttpGet]
        public async Task<IActionResult> GetAllSuggestedCommentsAsync([FromQuery]CommentParameters commentParameters) => 
            Ok(await suggestCommentRepository.GetAllAsync(commentParameters));

        [Route("DoesNextSuggestedCommentsPageExist")]
        [HttpGet]
        public async Task<IActionResult> DoesNextSuggestedCommentsPageExistAsync([FromQuery] CommentParameters commentParameters)
        {
            var doesExist = await paginationService.DoesNextSuggestedCommentsPageExistAsync(commentParameters);
            return Ok(doesExist);
        }

        [Route("Suggest")]
        [HttpPost]
        public async Task<IActionResult> SuggestCommentAsync([FromBody] SuggestCommentDto model)
        {
            var suggestedComment = await suggestCommentRepository.CreateAsync(new SuggestedComment
            {
                Content = model.Content, CreatedBy = model.CreatedBy, DiscussionId = model.DiscussionId,
                CreatedDate = DateTime.UtcNow, Id = Guid.NewGuid(), RepliedOnCommentContent = model.RepliedOnCommentContent,
                RepliedOnCommentCreatedBy = model.RepliedOnCommentCreatedBy, RepliedOnCommentId = model.RepliedOnCommentId
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
                DiscussionId = suggestedComment.DiscussionId,
                RepliedOnCommentContent = suggestedComment.RepliedOnCommentContent,
                RepliedOnCommentCreatedBy = suggestedComment.RepliedOnCommentCreatedBy,
                RepliedOnCommentId = suggestedComment.RepliedOnCommentId
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

        [Route("GetSuggestedCommentsByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedCommentsByUserNameAsync(string userName, [FromQuery] CommentParameters commentParameters)
            => Ok(await suggestCommentRepository.GetByUserNameAsync(userName, commentParameters));

        [Route("DoesNextSuggestedCommentsByUserNamePageExist/{userName}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextSuggestedCommentsByUserNamePageExistAsync(string userName, [FromQuery] CommentParameters commentParameters)
        {
            bool doesExist =
                await paginationService.DoesNextSuggestedCommentsByUserNamePageExistAsync(userName, commentParameters);
            return Ok(doesExist);
        }

        [Route("DeleteAllSuggestedCommentsByUserName/{userName}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAllSuggestedCommentsByUserNameAsync(string userName)
        {
            await suggestCommentRepository.DeleteByUserNameAsync(userName);
            return Ok();
        }
    }
}
