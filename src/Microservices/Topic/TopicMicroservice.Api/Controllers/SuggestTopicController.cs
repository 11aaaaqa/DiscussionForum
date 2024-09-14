using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopicMicroservice.Api.Database;
using TopicMicroservice.Api.DTOs;
using TopicMicroservice.Api.Models;

namespace TopicMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestTopicController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<SuggestTopicController> logger;

        public SuggestTopicController(ApplicationDbContext context, ILogger<SuggestTopicController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        [Route("SuggestTopic")]
        [HttpPost]
        public async Task<IActionResult> SuggestTopicAsync([FromBody] TopicDto model)
        {
            var topic = await context.Topics.SingleOrDefaultAsync(x => x.Name == model.Name);
            if (topic == null)
            {
                var suggestedTopic = await context.SuggestedTopics.SingleOrDefaultAsync(x => x.Name == model.Name);
                if (suggestedTopic != null)
                {
                    logger.LogError("Topic wasn't suggested because current topic name already suggested");
                    return BadRequest(new ErrorResponse{Reason = "Тема уже предложена" });
                }

                await context.SuggestedTopics.AddAsync(new SuggestedTopic
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    PostsCount = 0, 
                    SuggestedBy = model.SuggestedBy
                });
                await context.SaveChangesAsync();
                logger.LogInformation("Topic was successfully suggested");
                return Ok();
            }
            logger.LogError("Topic wasn't suggested because current topic name already exists");
            return BadRequest(new ErrorResponse{Reason = "Тема уже существует" });
        }

        [Route("RejectSuggestedTopic/{id}")]
        [HttpDelete]
        public async Task<IActionResult> RejectSuggestedTopicAsync(Guid id)
        {
            context.SuggestedTopics.Remove(new SuggestedTopic { Id = id });
            await context.SaveChangesAsync();
            logger.LogInformation("Suggested topic was successfully rejected");
            return Ok();
        }

        [Route("AcceptSuggestedTopic/{id}")]
        [HttpDelete]
        public async Task<IActionResult> AcceptSuggestedTopicAsync(Guid id)
        {
            var topic = await context.SuggestedTopics.SingleAsync(x => x.Id == id);
            context.SuggestedTopics.Remove(topic);
            await context.Topics.AddAsync(new Topic{Id = topic.Id, Name = topic.Name, PostsCount = topic.PostsCount});
            await context.SaveChangesAsync();
            logger.LogInformation("Suggested topic was successfully accepted");
            return Ok();
        }

        [Route("GetAllSuggestedTopics")]
        [HttpGet]
        public async Task<IActionResult> GetAllSuggestedTopicsAsync() =>
            Ok(await context.SuggestedTopics.ToListAsync());
    }
}
