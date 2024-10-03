namespace TopicMicroservice.Api.Services
{
    public interface ITopicService
    {
        Task<bool> DoesAllTopicsHaveNextPage(int pageSize, int pageNumber);
        Task<bool> DoesAllTopicsHaveNextPage(int pageSize, int pageNumber, string searchingQuery);
    }
}
