using DiscussionMicroservice.Api.Models;

namespace DiscussionMicroservice.Api.Services
{
    public interface IGetAllDiscussionsService
    {
        Task<List<Discussion>> GetAllDiscussionsSortedByNovelty(DiscussionParameters discussionParameters, string topicName);
        Task<List<Discussion>> GetAllDiscussionsSortedByPopularity(DiscussionParameters discussionParameters, string topicName);
    }
}
