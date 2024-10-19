using Web.MVC.Models.ApiResponses.Bookmark;

namespace Web.MVC.Models.ViewModels.Bookmark
{
    public class BookmarkViewModel
    {
        public List<BookmarkResponseModel> Bookmarks { get; set; }
        public string UserName { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NextPageNumber { get; set; }
        public int PreviousPageNumber { get; set; }
        public bool DoesNextPageExist { get; set; }
        public string? SearchingQuery { get; set; }
    }
}
