namespace BookmarkMicroservice.Api.DTOs
{
    public class BookmarkDto
    {
        public Guid UserId { get; set; }
        public Guid DiscussionId { get; set; }
        public string DiscussionTitle { get; set; }
    }
}
