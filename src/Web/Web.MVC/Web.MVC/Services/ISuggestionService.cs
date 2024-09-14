namespace Web.MVC.Services
{
    public interface ISuggestionService
    {
        Task<bool> DeleteSuggestedTopic(Guid suggestedTopicId);
        Task<bool> DeleteSuggestedDiscussion(Guid suggestedDiscussionId);
        Task<bool> DeleteSuggestedComment(Guid suggestedCommentId);
    }
}
