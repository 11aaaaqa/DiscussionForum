using TopicMicroservice.Api.Models;

namespace TopicMicroservice.Api.Services
{
    public interface IGetTopicsService
    {
        Task<List<Topic>> GetAllTopicSortedByNovelty(TopicParameters topicParameters);
        Task<List<Topic>> GetAllTopicSortedByPopularity(TopicParameters topicParameters);
    }
}
