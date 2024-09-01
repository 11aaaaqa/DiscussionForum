namespace CommentMicroservice.Api.DTOs
{
    public class SuggestCommentDto
    {
        public string CreatedBy { get; set; }
        public string Content { get; set; }
        public Guid DiscussionId { get; set; }
    }
}
