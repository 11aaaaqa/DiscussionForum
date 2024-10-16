namespace Web.MVC.Models.ApiResponses.CommentsResponses
{
    public class CommentResponse
    {
        public Guid Id { get; set; }
        public Guid DiscussionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Content { get; set; }
        public Guid? RepliedOnCommentId { get; set; }
        public string? RepliedOnCommentCreatedBy { get; set; }
        public string? RepliedOnCommentContent { get; set; }
    }
}
