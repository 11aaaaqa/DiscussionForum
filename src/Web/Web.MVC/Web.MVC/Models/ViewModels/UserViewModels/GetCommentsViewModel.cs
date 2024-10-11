using Web.MVC.Models.ApiResponses.CommentsResponses;

namespace Web.MVC.Models.ViewModels.UserViewModels
{
    public class GetCommentsViewModel
    {
        public List<CommentResponse> Comments { get; set; }
        public int PageSize { get; set; }
        public bool DoesNextPageExist { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NextPageNumber { get; set; }
        public int PreviousPageNumber { get; set; }
    }
}
