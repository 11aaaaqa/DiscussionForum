using DiscussionMicroservice.Api.Database;
using MassTransit;
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

        [Route("DeleteDiscussionById/{discussionId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteDiscussionByIdAsync(Guid discussionId)
        {
            var discussion = await context.Discussions.SingleOrDefaultAsync(x => x.Id == discussionId);
            if (discussion is null)
                return BadRequest();

            context.Remove(discussion);
            await context.SaveChangesAsync();

            await publishEndpoint.Publish<IDiscussionDeleted>(new { discussion.TopicName });
            await publishEndpoint.Publish<DiscussionDeleted>(new { DiscussionId = discussionId });

            return Ok();
        }
    }
}
