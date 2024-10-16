namespace CommentMicroservice.Api.DTOs
{
    public class CreateCommentDto
    {
        public Guid DiscussionId { get; set; }
        public string CreatedBy { get; set; }
        public string Content { get; set; }
        public Guid? RepliedOnCommentId { get; set; }
        public string? RepliedOnCommentCreatedBy { get; set; }
        public string? RepliedOnCommentContent { get; set; }
    }
}
