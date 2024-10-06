using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionMicroservice.Api.Services
{
    public class CheckForNextDiscussionsPageExistingService : ICheckForNextDiscussionsPageExisting
    {
        private readonly ApplicationDbContext context;

        public CheckForNextDiscussionsPageExistingService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> DoesNextDiscussionsPageExistSearchingAsync(DiscussionParameters discussionParameters, string searchingQuery,
            string topicName)
        {
            int totalDiscussionsCount = await context.Discussions.Where(x => x.TopicName == topicName)
                .Where(x => x.Title.Contains(searchingQuery)).CountAsync();
            int totalGettingDiscussionsCount = discussionParameters.PageSize * discussionParameters.PageNumber;
            int pageStartCount = totalGettingDiscussionsCount - discussionParameters.PageSize;
            bool doesExist = (totalDiscussionsCount > pageStartCount);
            return doesExist;
        }

        public async Task<bool> DoesNextDiscussionsPageExistAsync(DiscussionParameters discussionParameters, string topicName)
        {
            int totalDiscussionsCount = await context.Discussions.Where(x => x.TopicName == topicName).CountAsync();
            int totalGettingDiscussionsCount = discussionParameters.PageSize * discussionParameters.PageNumber;
            int pageStartCount = totalGettingDiscussionsCount - discussionParameters.PageSize;
            bool doesExist = (totalDiscussionsCount > pageStartCount);
            return doesExist;
        }

        public async Task<bool> DoesNextAllDiscussionsPageExistAsync(DiscussionParameters discussionParameters)
        {
            int totalDiscussionsCount = await context.Discussions.CountAsync();
            int totalGettingDiscussionsCount = discussionParameters.PageSize * discussionParameters.PageNumber;
            int pageStartCount = totalGettingDiscussionsCount - discussionParameters.PageSize;
            bool doesExist = (totalDiscussionsCount > pageStartCount);
            return doesExist;
        }

        public async Task<bool> DoesNextDiscussionsForTodayPageExistAsync(DiscussionParameters discussionParameters)
        {
            int totalDiscussionsCount = await context.Discussions.Where(x => x.CreatedAt == DateOnly.FromDateTime(DateTime.UtcNow)).CountAsync();
            int totalGettingDiscussionsCount = discussionParameters.PageSize * discussionParameters.PageNumber;
            int pageStartCount = totalGettingDiscussionsCount - discussionParameters.PageSize;
            bool doesExist = (totalDiscussionsCount > pageStartCount);
            return doesExist;
        }

        public async Task<bool> DoesNextDiscussionsForWeekPageExistAsync(DiscussionParameters discussionParameters)
        {
            int totalDiscussionsCount = await context.Discussions
                .Where(x => x.CreatedAt >= DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-7)).CountAsync();
            int totalGettingDiscussionsCount = discussionParameters.PageSize * discussionParameters.PageNumber;
            int pageStartCount = totalGettingDiscussionsCount - discussionParameters.PageSize;
            bool doesExist = (totalDiscussionsCount > pageStartCount);
            return doesExist;
        }

        public async Task<bool> DoesNextDiscussionsForMonthPageExistAsync(DiscussionParameters discussionParameters)
        {
            int totalDiscussionsCount = await context.Discussions
                .Where(x => x.CreatedAt >= DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-1)).CountAsync();
            int totalGettingDiscussionsCount = discussionParameters.PageSize * discussionParameters.PageNumber;
            int pageStartCount = totalGettingDiscussionsCount - discussionParameters.PageSize;
            bool doesExist = (totalDiscussionsCount > pageStartCount);
            return doesExist;
        }
    }
}
