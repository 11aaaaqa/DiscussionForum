using Microsoft.EntityFrameworkCore;
using TopicMicroservice.Api.Database;

namespace TopicMicroservice.Api.Services
{
    public class TopicService : ITopicService
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
    }
}
