namespace MessageBus.Messages
{
    public interface IUserSuggestedDiscussion
    {
        public Guid SuggestedDiscussionId { get; set; }
        public string CreatedBy { get; set; }
    }
}
