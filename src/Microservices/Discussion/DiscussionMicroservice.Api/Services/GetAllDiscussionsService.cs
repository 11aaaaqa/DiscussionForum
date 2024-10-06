using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionMicroservice.Api.Services
{
    public class GetAllDiscussionsService : IGetAllDiscussionsService
    {
        private readonly ApplicationDbContext context;

        public GetAllDiscussionsService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Discussion>> GetAllDiscussionsSortedByNovelty(DiscussionParameters discussionParameters)
        {
            var discussions = await context.Discussions.OrderByDescending(x => x.CreatedAt)
                .Skip((discussionParameters.PageNumber - 1) * discussionParameters.PageSize)
                .Take(discussionParameters.PageSize).ToListAsync();
            return discussions;
        }

        public async Task<List<Discussion>> GetAllDiscussionsSortedByPopularityForToday(DiscussionParameters discussionParameters)
        {
            var discussions = await context.Discussions
                .Where(x => x.CreatedAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .OrderByDescending(x => x.Rating)
                .Skip((discussionParameters.PageNumber - 1) * discussionParameters.PageSize)
                .Take(discussionParameters.PageSize).ToListAsync();
            return discussions;
        }

        public async Task<List<Discussion>> GetAllDiscussionsSortedByPopularityForWeek(DiscussionParameters discussionParameters)
        {
            var discussions = await context.Discussions
                .Where(x => x.CreatedAt >= DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-7))
                .OrderByDescending(x => x.Rating)
                .Skip((discussionParameters.PageNumber - 1) * discussionParameters.PageSize)
                .Take(discussionParameters.PageSize).ToListAsync();
            return discussions;
        }

        public async Task<List<Discussion>> GetAllDiscussionsSortedByPopularityForMonth(DiscussionParameters discussionParameters)
        {
            var discussions = await context.Discussions
                .Where(x => x.CreatedAt >= DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-1))
                .OrderByDescending(x => x.Rating)
                .Skip((discussionParameters.PageNumber - 1) * discussionParameters.PageSize)
                .Take(discussionParameters.PageSize).ToListAsync();
            return discussions;
        }

        public async Task<List<Discussion>> GetAllDiscussionsSortedByPopularityForAllTime(DiscussionParameters discussionParameters)
        {
            var discussions = await context.Discussions
                .OrderByDescending(x => x.Rating)
                .Skip((discussionParameters.PageNumber - 1) * discussionParameters.PageSize)
                .Take(discussionParameters.PageSize).ToListAsync();
            return discussions;
        }
    }
}
