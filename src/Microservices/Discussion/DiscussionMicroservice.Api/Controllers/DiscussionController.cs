using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.DTOs;
using DiscussionMicroservice.Api.Models;
using MassTransit;
using MessageBus.Messages;
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
        public async Task<IActionResult> GetDiscussionsByTopicNameAsync(
            [FromQuery] DiscussionParameters discussionParameters, string topicName)
        {
            var discussions = await context.Discussions.Where(x => x.TopicName == topicName)
                .Skip(discussionParameters.PageSize * (discussionParameters.PageNumber - 1))
                .Take(discussionParameters.PageSize).ToListAsync();
            return Ok(discussions);
        }

        [Route("FindDiscussionsByTopicNameBySearchingString")]
        [HttpGet]
        public async Task<IActionResult> FindDiscussionsByTopicNameAsync(
            [FromQuery] DiscussionParameters discussionParameters, string topicName, string searchingString)
        {
            var discussions = await context.Discussions.Where(x => x.TopicName == topicName)
                .Where(x => x.Title.Contains(searchingString))
                .Skip(discussionParameters.PageSize * (discussionParameters.PageNumber - 1))
                .Take(discussionParameters.PageSize).ToListAsync();
            return Ok(discussions);
        }

        [Route("DoesNextDiscussionsPageExistSearching")]
        [HttpGet]
        public async Task<IActionResult> DoesNextDiscussionsPageExistSearchingAsync([FromQuery]DiscussionParameters discussionParameters, string searchingQuery,
            string topicName)
        {
            int totalDiscussionsCount = await context.Discussions.Where(x => x.TopicName == topicName)
                .Where(x => x.Title.Contains(searchingQuery)).CountAsync();
            int totalGettingDiscussionsCount = discussionParameters.PageSize * discussionParameters.PageNumber;
            int pageStartCount = totalGettingDiscussionsCount - discussionParameters.PageSize;
            bool doesExist = (totalDiscussionsCount > pageStartCount);
            return Ok(doesExist);
        }

        [Route("DoesNextDiscussionsPageExist")]
        [HttpGet]
        public async Task<IActionResult> DoesNextDiscussionsPageExistAsync([FromQuery]DiscussionParameters discussionParameters, string topicName)
        {
            int totalDiscussionsCount = await context.Discussions.Where(x => x.TopicName == topicName).CountAsync();
            int totalGettingDiscussionsCount = discussionParameters.PageSize * discussionParameters.PageNumber;
            int pageStartCount = totalGettingDiscussionsCount - discussionParameters.PageSize;
            bool doesExist = (totalDiscussionsCount > pageStartCount);
            return Ok(doesExist);
        }

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
        [HttpPatch]
        public async Task<IActionResult> IncreaseDiscussionRatingByOneAsync([FromBody] IncreaseDiscussionRatingByOneDto model)
        {
            var discussion = await context.Discussions.SingleOrDefaultAsync(x => x.Id == model.DiscussionId);
            if(discussion is null) return BadRequest();

            var isUserAlreadyIncreased = discussion.UsersIncreasedRating.Contains(model.UserNameIncreasedBy);
            if (isUserAlreadyIncreased) return BadRequest();

            discussion.UsersIncreasedRating.Add(model.UserNameIncreasedBy);
            discussion.Rating += 1;

            context.Discussions.Update(discussion);
            await context.SaveChangesAsync();

            return Ok();
        }

        [Route("DecreaseDiscussionRatingByOne")]
        [HttpPatch]
        public async Task<IActionResult> DecreaseDiscussionRatingByOneAsync([FromBody] DecreaseDiscussionRatingByOneMethodDto model)
        {
            var discussion = await context.Discussions.SingleOrDefaultAsync(x => x.Id == model.DiscussionId);
            if (discussion is null) return BadRequest();

            var isUserAlreadyDecreased = discussion.UsersDecreasedRating.Contains(model.UserNameDecreasedBy);
            if (isUserAlreadyDecreased) return BadRequest();

            discussion.UsersDecreasedRating.Add(model.UserNameDecreasedBy);
            discussion.Rating -= 1;

            context.Discussions.Update(discussion);
            await context.SaveChangesAsync();

            return Ok();
        }

        [Route("CreateDiscussion")]
        [HttpPost]
        public async Task<IActionResult> CreateDiscussionAsync([FromBody] CreateDiscussionDto model)
        {
            var discussion = new Discussion
            {
                Content = model.Content,
                CreatedBy = model.CreatedBy,
                Title = model.Title,
                TopicName = model.TopicName,
                Id = Guid.NewGuid(),
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                Rating = 0,
                UsersDecreasedRating = new List<string>(),
                UsersIncreasedRating = new List<string>()
            };
            await context.Discussions.AddAsync(discussion);
            await context.SaveChangesAsync();

            await publishEndpoint.Publish<ISuggestedDiscussionAccepted>(new
            {
                AcceptedDiscussionId = discussion.Id,
                discussion.CreatedBy,
                TopicName = discussion.TopicName
            });
            return Ok(discussion.Id);
        }
    }
}
