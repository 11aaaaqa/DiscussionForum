namespace MessageBus.Messages
{
    public interface IDiscussionDeleted
    {
        public Guid DiscussionId { get; set; }
        public string TopicName { get; set; }
    }
}
