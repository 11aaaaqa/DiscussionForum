namespace BookmarkMicroservice.Api.Models
{
    public class Bookmark
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DiscussionId { get; set; }
        public string DiscussionTitle { get; set; }
    }
}