using DiscussionMicroservice.Api.Models;

namespace DiscussionMicroservice.Api.Services
{
    public interface ICheckForNextDiscussionsPageExisting
    {
        Task<bool> DoesNextDiscussionsPageExistSearchingAsync(DiscussionParameters discussionParameters, string searchingQuery, string topicName);
        Task<bool> DoesNextDiscussionsPageExistAsync(DiscussionParameters discussionParameters, string topicName);
        Task<bool> DoesNextAllDiscussionsPageExistAsync(DiscussionParameters discussionParameters);
        Task<bool> DoesNextDiscussionsForTodayPageExistAsync(DiscussionParameters discussionParameters);
        Task<bool> DoesNextDiscussionsForWeekPageExistAsync(DiscussionParameters discussionParameters);
        Task<bool> DoesNextDiscussionsForMonthPageExistAsync(DiscussionParameters discussionParameters);
    }
}
