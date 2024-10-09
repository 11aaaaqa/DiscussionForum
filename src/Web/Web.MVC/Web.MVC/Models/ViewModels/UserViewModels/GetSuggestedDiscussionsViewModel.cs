using Web.MVC.Models.ApiResponses.Discussion;

namespace Web.MVC.Models.ViewModels.UserViewModels
{
    public class GetSuggestedDiscussionsViewModel
    {
        public List<DiscussionResponse> SuggestedDiscussions { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public int PreviousPageNumber { get; set; }
        public int NextPageNumber { get; set; }
        public bool DoesNextPageExist { get; set; }
    }
}
