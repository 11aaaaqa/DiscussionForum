using DiscussionMicroservice.Api.Models;

namespace DiscussionMicroservice.Api.Services
{
    public interface IGetAllDiscussionsService
    {
        Task<List<Discussion>> GetAllDiscussionsSortedByNovelty(DiscussionParameters discussionParameters);
        Task<List<Discussion>> GetAllDiscussionsSortedByPopularityForToday(DiscussionParameters discussionParameters);
        Task<List<Discussion>> GetAllDiscussionsSortedByPopularityForWeek(DiscussionParameters discussionParameters);
        Task<List<Discussion>> GetAllDiscussionsSortedByPopularityForMonth(DiscussionParameters discussionParameters);
        Task<List<Discussion>> GetAllDiscussionsSortedByPopularityForAllTime(DiscussionParameters discussionParameters);
    }
}
