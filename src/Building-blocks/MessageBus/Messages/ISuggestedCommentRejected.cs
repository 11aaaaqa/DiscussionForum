namespace MessageBus.Messages
{
    public interface ISuggestedCommentRejected
    {
        public Guid RejectedCommentId { get; set; }
        public string CreatedBy { get; set; }
    }
}
