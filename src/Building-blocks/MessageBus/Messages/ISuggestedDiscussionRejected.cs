namespace MessageBus.Messages
{
    public interface ISuggestedDiscussionRejected
    {
        public Guid SuggestedDiscussionId { get; set; }
        public string SuggestedBy { get; set; }
    }
}
