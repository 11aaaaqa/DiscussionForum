namespace MessageBus.Messages
{
    public interface IDiscussionAdded
    {
        uint DiscussionCount { get; set; }
        string TopicName { get; set; }
    }
}
