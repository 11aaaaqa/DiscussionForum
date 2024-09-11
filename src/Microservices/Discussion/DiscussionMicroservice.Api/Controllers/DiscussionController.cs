using DiscussionMicroservice.Api.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscussionMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscussionController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public DiscussionController(ApplicationDbContext context)
        {
            this.context = context;
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
            return Ok();
        }
    }
}
