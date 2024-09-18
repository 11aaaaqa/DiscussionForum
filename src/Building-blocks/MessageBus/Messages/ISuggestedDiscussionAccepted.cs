namespace MessageBus.Messages
{
    public interface ISuggestedDiscussionAccepted
    {
        public Guid AcceptedDiscussionId { get; set; }
        public string CreatedBy { get; set; }
        public string TopicName { get; set; }
    }
}
