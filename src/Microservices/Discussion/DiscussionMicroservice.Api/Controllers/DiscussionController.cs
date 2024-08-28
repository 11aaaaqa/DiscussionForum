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
    }
}
