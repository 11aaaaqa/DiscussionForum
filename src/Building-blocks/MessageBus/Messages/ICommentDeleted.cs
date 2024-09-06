namespace MessageBus.Messages
{
    public interface ICommentDeleted
    {
        public Guid CommentId { get; set; }
        public string CommentCreatedBy { get; set; }
    }
}
