using Web.MVC.Models.ApiResponses.Discussion;

namespace Web.MVC.Models.ViewModels.ModeratorViewModels
{
    public class GetSuggestedDiscussionsByUserNameViewModel
    {
        public List<SuggestedDiscussionResponse> SuggestedDiscussions { get; set; }
        public bool DoesNextPageExist { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NextPageNumber { get; set; }
        public int PreviousPageNumber { get; set; }
        public string SuggestedByUserName { get; set; }
    }
}
