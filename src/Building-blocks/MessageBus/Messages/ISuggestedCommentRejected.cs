namespace MessageBus.Messages
{
    public interface ISuggestedCommentRejected
    {
        public Guid AcceptedCommentId { get; set; }
        public string CreatedBy { get; set; }
    }
}
