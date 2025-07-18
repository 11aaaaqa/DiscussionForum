﻿using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.DTOs;
using DiscussionMicroservice.Api.Models;
using DiscussionMicroservice.Api.Services;
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
        private readonly IGetAllDiscussionsService getAllDiscussionsService;
        private readonly ICheckForNextDiscussionsPageExisting checkForNextPageExisting;
        private readonly ApplicationDbContext context;
        private readonly IPublishEndpoint publishEndpoint;

        public DiscussionController(ApplicationDbContext context, IPublishEndpoint publishEndpoint, IGetAllDiscussionsService getAllDiscussionsService,
            ICheckForNextDiscussionsPageExisting checkForNextPageExisting)
        {
            this.context = context;
            this.publishEndpoint = publishEndpoint;
            this.getAllDiscussionsService = getAllDiscussionsService;
            this.checkForNextPageExisting = checkForNextPageExisting;
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
                .Where(x => x.Title.ToLower().Contains(searchingString.ToLower()))
                .Skip(discussionParameters.PageSize * (discussionParameters.PageNumber - 1))
                .Take(discussionParameters.PageSize).ToListAsync();
            return Ok(discussions);
        }

        [Route("DoesNextDiscussionsPageExistSearching")]
        [HttpGet]
        public async Task<IActionResult> DoesNextDiscussionsPageExistSearchingAsync([FromQuery]DiscussionParameters discussionParameters, string searchingQuery,
            string topicName)
        {
            bool doesExist = await checkForNextPageExisting.DoesNextDiscussionsPageExistSearchingAsync(
                discussionParameters, searchingQuery, topicName);
            return Ok(doesExist);
        }

        [Route("DoesNextDiscussionsPageExist")]
        [HttpGet]
        public async Task<IActionResult> DoesNextDiscussionsPageExistAsync([FromQuery]DiscussionParameters discussionParameters, string topicName)
        {
            bool doesExist =
                await checkForNextPageExisting.DoesNextDiscussionsPageExistAsync(discussionParameters, topicName);
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

        [Route("DoesNextAllDiscussionsPageExist")]
        [HttpGet]
        public async Task<IActionResult> DoesNextDiscussionsPageExistAsync([FromQuery] DiscussionParameters discussionParameters)
        {
            bool doesExist = await checkForNextPageExisting.DoesNextAllDiscussionsPageExistAsync(discussionParameters);
            return Ok(doesExist);
        }

        [Route("DoesNextDiscussionsForTodayPageExist")]
        [HttpGet]
        public async Task<IActionResult> DoesNextDiscussionsForTodayPageExistAsync([FromQuery] DiscussionParameters discussionParameters)
        {
            bool doesExist =
                await checkForNextPageExisting.DoesNextDiscussionsForTodayPageExistAsync(discussionParameters);
            return Ok(doesExist);
        }

        [Route("GetAllDiscussionsSortedByNovelty")]
        [HttpGet]
        public async Task<IActionResult> GetAllDiscussionsSortedByNoveltyAsync([FromQuery] DiscussionParameters discussionParameters)
        {
            var discussions =
                await getAllDiscussionsService.GetAllDiscussionsSortedByNovelty(discussionParameters);
            return Ok(discussions);
        }

        [Route("GetAllDiscussionsSortedByPopularityForToday")]
        [HttpGet]
        public async Task<IActionResult> GetAllDiscussionsSortedByPopularityForTodayAsync([FromQuery] DiscussionParameters discussionParameters)
        {
            var discussions =
                await getAllDiscussionsService.GetAllDiscussionsSortedByPopularityForToday(discussionParameters);
            return Ok(discussions);
        }

        [Route("GetAllDiscussionsSortedByPopularityForWeek")]
        [HttpGet]
        public async Task<IActionResult> GetAllDiscussionsSortedByPopularityForWeekAsync([FromQuery] DiscussionParameters discussionParameters)
        {
            var discussions =
                await getAllDiscussionsService.GetAllDiscussionsSortedByPopularityForWeek(discussionParameters);
            return Ok(discussions);
        }

        [Route("GetAllDiscussionsSortedByPopularityForMonth")]
        [HttpGet]
        public async Task<IActionResult> GetAllDiscussionsSortedByPopularityForMonthAsync([FromQuery] DiscussionParameters discussionParameters)
        {
            var discussions =
                await getAllDiscussionsService.GetAllDiscussionsSortedByPopularityForMonth(discussionParameters);
            return Ok(discussions);
        }

        [Route("GetAllDiscussionsSortedByPopularityForAllTime")]
        [HttpGet]
        public async Task<IActionResult> GetAllDiscussionsSortedByPopularityForAllTimeAsync([FromQuery] DiscussionParameters discussionParameters)
        {
            var discussions =
                await getAllDiscussionsService.GetAllDiscussionsSortedByPopularityForAllTime(discussionParameters);
            return Ok(discussions);
        }

        [Route("DoesNextDiscussionsForWeekPageExist")]
        [HttpGet]
        public async Task<IActionResult> DoesNextDiscussionsForWeekPageExistAsync([FromQuery] DiscussionParameters discussionParameters)
        {
            var doesExist =
                await checkForNextPageExisting.DoesNextDiscussionsForWeekPageExistAsync(discussionParameters);
            return Ok(doesExist);
        }

        [Route("DoesNextDiscussionsForMonthPageExist")]
        [HttpGet]
        public async Task<IActionResult> DoesNextDiscussionsForMonthPageExistAsync([FromQuery] DiscussionParameters discussionParameters)
        {
            var doesExist =
                await checkForNextPageExisting.DoesNextDiscussionsForMonthPageExistAsync(discussionParameters);
            return Ok(doesExist);
        }

        [Route("GetDiscussionsByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetDiscussionsByUserNameAsync(string userName, [FromQuery] DiscussionParameters discussionParameters)
        {
            var discussions = await context.Discussions.Where(x => x.CreatedBy == userName)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(discussionParameters.PageSize * (discussionParameters.PageNumber - 1))
                .Take(discussionParameters.PageSize).ToListAsync();
            return Ok(discussions);
        }

        [Route("DoesNextDiscussionsByUserNamePageExist/{userName}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextDiscussionsByUserNamePageExistAsync(string userName, [FromQuery] DiscussionParameters discussionParameters)
        {
            int totalDiscussionsCount = await context.Discussions.Where(x => x.CreatedBy == userName).CountAsync();
            int totalRequestedDiscussions = discussionParameters.PageSize * discussionParameters.PageNumber;
            int startRequestedDiscussionsCount = totalRequestedDiscussions - discussionParameters.PageSize;
            bool doesExist = totalDiscussionsCount > startRequestedDiscussionsCount;
            return Ok(doesExist);
        }
    }
}
