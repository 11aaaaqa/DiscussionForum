namespace MessageBus.Messages
{
    public interface ISuggestedCommentAccepted
    {
        public Guid AcceptedCommentId { get; set; }
        public string CreatedBy { get; set; }
    }
}
