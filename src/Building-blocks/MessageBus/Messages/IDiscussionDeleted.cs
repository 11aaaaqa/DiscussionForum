namespace MessageBus.Messages
{
    public interface IDiscussionDeleted
    {
        public string TopicName { get; set; }
        public Guid DiscussionId { get; set; }
    }
}
