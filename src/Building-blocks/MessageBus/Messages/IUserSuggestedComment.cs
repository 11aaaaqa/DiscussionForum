namespace MessageBus.Messages
{
    public interface IUserSuggestedComment
    {
        public Guid SuggestedCommentId { get; set; }
        public string SuggestedBy { get; set; }
    }
}
