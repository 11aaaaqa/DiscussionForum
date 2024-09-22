using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.Models;
using MassTransit;
using MassTransit.Transports.Fabric;
using MessageBus.Messages;
using MessageBus.Messages.BanMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscussionMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscussionController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IPublishEndpoint publishEndpoint;

        public DiscussionController(ApplicationDbContext context, IPublishEndpoint publishEndpoint)
        {
            this.context = context;
            this.publishEndpoint = publishEndpoint;
        }

        [Route("GetDiscussionsByTopicName")]
        [HttpGet]
        public async Task<IActionResult> GetDiscussionsByTopicNameAsync(string topicName) =>
            Ok(await context.Discussions.Where(x => x.TopicName == topicName).ToListAsync());

        [Route("GetDiscussionById")]
        [HttpGet]
        public async Task<IActionResult> GetDiscussionByIdAsync(Guid id)
        {
            var discussion = await context.Discussions.SingleOrDefaultAsync(x => x.Id == id);
            if (discussion == null)
                return BadRequest();
            
            return Ok(discussion);
        }

        [Route("GetDiscussionsByIds")]
        [HttpGet]
        public async Task<IActionResult> GetDiscussionsByIdsAsync([FromQuery(Name = "ids[]")] params Guid[] ids)
        {
            var discussions = new List<Discussion>();
            foreach (var id in ids)
            {
                var discussion = await context.Discussions.SingleOrDefaultAsync(x => x.Id == id);
                if(discussion is not null) discussions.Add(discussion);
            }

            return Ok(discussions);
        }

        [Route("DeleteDiscussionById/{discussionId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteDiscussionByIdAsync(Guid discussionId)
        {
            var discussion = await context.Discussions.SingleOrDefaultAsync(x => x.Id == discussionId);
            if (discussion is null)
                return BadRequest();
            
            context.Remove(discussion);
            await context.SaveChangesAsync();

            await publishEndpoint.Publish<IDiscussionDeleted>(new
            {
                DiscussionId = discussion.Id, discussion.TopicName, UserNameDiscussionCreatedBy = discussion.CreatedBy
            });

            return Ok();
        }

        [Route("IncreaseDiscussionRatingByOne")]
        [HttpPut]
        public async Task<IActionResult> IncreaseDiscussionRatingByOneAsync(Guid discussionId, string userNameIncreasedBy)
        {
            var discussion = await context.Discussions.SingleOrDefaultAsync(x => x.Id == discussionId);
            if(discussion is null) return BadRequest();

            var isUserAlreadyIncreased = discussion.UsersIncreasedRating.Contains(userNameIncreasedBy);
            if (isUserAlreadyIncreased) return BadRequest();

            discussion.UsersIncreasedRating.Add(userNameIncreasedBy);
            discussion.Rating += 1;
            return Ok();
        }

        [Route("DecreaseDiscussionRatingByOne")]
        [HttpPut]
        public async Task<IActionResult> DecreaseDiscussionRatingByOneAsync(Guid discussionId, string userNameDecreasedBy)
        {
            var discussion = await context.Discussions.SingleOrDefaultAsync(x => x.Id == discussionId);
            if (discussion is null) return BadRequest();

            var isUserAlreadyDecreased = discussion.UsersDecreasedRating.Contains(userNameDecreasedBy);
            if (isUserAlreadyDecreased) return BadRequest();

            discussion.UsersDecreasedRating.Add(userNameDecreasedBy);
            discussion.Rating -= 1;
            return Ok();
        }
    }
}
