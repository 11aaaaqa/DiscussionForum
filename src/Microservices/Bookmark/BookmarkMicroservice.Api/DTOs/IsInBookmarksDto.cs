namespace BookmarkMicroservice.Api.DTOs
{
    public class IsInBookmarksDto
    {
        public bool IsInBookmarks { get; set; }
        public Guid? BookmarkId { get; set; }
    }
}
