namespace MessageBus.Messages
{
    public interface IDiscussionAdded
    {
        int DiscussionCount { get; set; }
        string TopicName { get; set; }
    }
}
