namespace Web.MVC.Models.ApiResponses.Bookmark
{
    public class BookmarkResponseModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public Guid DiscussionId { get; set; }
        public string DiscussionTitle { get; set; }
    }
}
