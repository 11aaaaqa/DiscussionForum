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
        public async Task<List<Discussion>> GetAllDiscussionsSortedByNovelty(DiscussionParameters discussionParameters, string topicName)
        {
            var discussions = await context.Discussions.Where(x => x.TopicName == topicName)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(discussionParameters.PageSize * (discussionParameters.PageNumber - 1))
                .Take(discussionParameters.PageSize).ToListAsync();
            return discussions;
        }

        public async Task<List<Discussion>> GetAllDiscussionsSortedByPopularity(DiscussionParameters discussionParameters, string topicName)
        {
            var discussions = await context.Discussions.Where(x => x.TopicName == topicName)
                .OrderByDescending(x => x.Rating)
                .Skip(discussionParameters.PageSize * (discussionParameters.PageNumber - 1))
                .Take(discussionParameters.PageSize).ToListAsync();
            return discussions;
        }
    }
}
