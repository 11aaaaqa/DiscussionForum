using Microsoft.EntityFrameworkCore;
using TopicMicroservice.Api.Database;
using TopicMicroservice.Api.Models;

namespace TopicMicroservice.Api.Services
{
    public class TopicService : ITopicService, IGetTopicsService
    {
        private readonly ApplicationDbContext context;

        public TopicService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> DoesAllTopicsHaveNextPage(int pageSize, int pageNumber)
        {
            int totalTopicsCount = await context.Topics.CountAsync();
            int totalRequestedTopicsCount = pageNumber * pageSize;
            int startedOnCurrentPageTopicsCount = totalRequestedTopicsCount - pageSize;
            return (totalTopicsCount > startedOnCurrentPageTopicsCount);
        }

        public async Task<bool> DoesAllTopicsHaveNextPage(int pageSize, int pageNumber, string searchingQuery)
        {
            int totalTopicsCount = await context.Topics.Where(x => x.Name.Contains(searchingQuery)).CountAsync();
            int totalRequestedTopicsCount = pageNumber * pageSize;
            int startedOnCurrentPageTopicsCount = totalRequestedTopicsCount - pageSize;
            return (totalTopicsCount > startedOnCurrentPageTopicsCount);
        }

        public async Task<List<Topic>> GetAllTopicSortedByNovelty(TopicParameters topicParameters)
        {
            var topics = await context.Topics.OrderByDescending(x => x.CreatedAt)
                .Skip(topicParameters.PageSize * (topicParameters.PageNumber - 1)).Take(topicParameters.PageSize).ToListAsync();
            return topics;
        }

        public async Task<List<Topic>> GetAllTopicSortedByPopularity(TopicParameters topicParameters)
        {
            var topics = await context.Topics.OrderByDescending(x => x.PostsCount)
                .Skip(topicParameters.PageSize * (topicParameters.PageNumber - 1)).Take(topicParameters.PageSize).ToListAsync();
            return topics;
        }
    }
}
