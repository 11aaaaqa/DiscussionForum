namespace MessageBus.Messages
{
    public interface ICommentCreated
    {
        public Guid CommentId { get; set; }
        public string CreatedBy { get; set; }
    }
}
