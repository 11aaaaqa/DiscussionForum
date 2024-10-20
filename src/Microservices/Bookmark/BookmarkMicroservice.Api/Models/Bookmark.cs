namespace BookmarkMicroservice.Api.Models
{
    public class Bookmark
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public Guid DiscussionId { get; set; }
        public string DiscussionTitle { get; set; }
        public DateTime AddedAt { get; set; }
    }
}