namespace TopicMicroservice.Api.Services
{
    public interface ITopicService
    {
        Task<bool> DoesAllTopicsHaveNextPage(int pageSize, int pageNumber);
    }
}
