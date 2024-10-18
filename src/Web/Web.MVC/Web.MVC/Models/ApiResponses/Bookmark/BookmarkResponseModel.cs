namespace Web.MVC.Models.ApiResponses.Bookmark
{
    public class BookmarkResponseModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DiscussionId { get; set; }
        public string DiscussionTitle { get; set; }
    }
}
