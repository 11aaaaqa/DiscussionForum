using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.DTOs;
using DiscussionMicroservice.Api.Models;
using MassTransit;
using MassTransit.Transports.Fabric;
using MessageBus.Messages;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscussionMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestDiscussionController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IPublishEndpoint publishEndpoint;

        public SuggestDiscussionController(ApplicationDbContext context, IPublishEndpoint publishEndpoint)
        {
            this.context = context;
            this.publishEndpoint = publishEndpoint;
        }

        [Route("SuggestToCreate")]
        [HttpPost]
        public async Task<IActionResult> SuggestToCreateDiscussionAsync([FromBody] DiscussionDto model, string topicName)
        {
            var suggestedDiscussion = new SuggestedDiscussion
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Content = model.Content,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                Rating = 0,
                TopicName = topicName,
                CreatedBy = model.CreatedBy
            };
            await context.SuggestedDiscussions.AddAsync(suggestedDiscussion);
            await context.SaveChangesAsync();

            await publishEndpoint.Publish<IUserSuggestedDiscussion>(new { SuggestedDiscussionId = suggestedDiscussion.Id, suggestedDiscussion.CreatedBy});

            return Ok();
        }

        [Route("GetAllSuggestedDiscussions")]
        [HttpGet]
        public async Task<IActionResult> GetAllSuggestedDiscussionsAsync() => Ok(await context.SuggestedDiscussions.ToListAsync());

        [Route("RejectSuggestedDiscussion/{id}")]
        [HttpDelete]
        public async Task<IActionResult> RejectSuggestedDiscussionAsync(Guid id)
        {
            var suggestedDiscussion = await context.SuggestedDiscussions.SingleAsync(x => x.Id == id);
            context.SuggestedDiscussions.Remove(suggestedDiscussion);
            await context.SaveChangesAsync();

            await publishEndpoint.Publish<ISuggestedDiscussionRejected>(new { SuggestedDiscussionId = id, SuggestedBy = suggestedDiscussion.CreatedBy });

            return Ok();
        }

        [HttpDelete]
        [Route("AcceptSuggestedDiscussion/{id}")]
        public async Task<IActionResult> AcceptSuggestedDiscussionAsync(Guid id)
        {
            var acceptedDiscussion = await context.SuggestedDiscussions.SingleAsync(x => x.Id == id);

            context.SuggestedDiscussions.Remove(acceptedDiscussion);
            await context.Discussions.AddAsync(new Discussion
            {
                Id = acceptedDiscussion.Id,
                Content = acceptedDiscussion.Content,
                CreatedAt = acceptedDiscussion.CreatedAt,
                CreatedBy = acceptedDiscussion.CreatedBy,
                Rating = acceptedDiscussion.Rating,
                Title = acceptedDiscussion.Title,
                TopicName = acceptedDiscussion.TopicName
            });
            await context.SaveChangesAsync();

            await publishEndpoint.Publish<ISuggestedDiscussionAccepted>(new
            {
                AcceptedDiscussionId = acceptedDiscussion.Id, acceptedDiscussion.CreatedBy, TopicName = acceptedDiscussion.TopicName
            });

            return Ok();
        }

        [HttpGet]
        [Route("GetSuggestedDiscussionById")]
        public async Task<IActionResult> GetSuggestedDiscussionByIdAsync(Guid id)
        {
            var suggestedDiscussion = await context.SuggestedDiscussions.SingleOrDefaultAsync(x => x.Id == id);
            if (suggestedDiscussion == null)
                return BadRequest();
            
            return Ok(suggestedDiscussion);
        }

        [Route("GetSuggestedDiscussionsByIds")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedDiscussionsByIdsAsync([FromQuery(Name = "ids[]")] params Guid[] ids)
        {
            var suggestedDiscussions = new List<SuggestedDiscussion>();
            foreach (var id in ids)
            {
                var suggestedDiscussion = await context.SuggestedDiscussions.SingleOrDefaultAsync(x => x.Id == id);
                if (suggestedDiscussion is not null) suggestedDiscussions.Add(suggestedDiscussion);
            }

            return Ok(suggestedDiscussions);
        }

        [Route("GetSuggestedDiscussionsByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedDiscussionsByUserNameAsync(string userName)
        {
            var suggestedTopics = await context.SuggestedDiscussions.Where(x => x.CreatedBy == userName).ToListAsync();
            return Ok(suggestedTopics);
        }
    }
}
