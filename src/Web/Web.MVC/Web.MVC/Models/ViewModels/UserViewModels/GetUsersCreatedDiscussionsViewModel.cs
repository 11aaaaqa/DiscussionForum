using Web.MVC.Models.ApiResponses.Discussion;

namespace Web.MVC.Models.ViewModels.UserViewModels
{
    public class GetUsersCreatedDiscussionsViewModel
    {
        public List<DiscussionResponse> CreatedDiscussions { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public int PreviousPageNumber { get; set; }
        public int NextNumber { get; set; }
        public bool DoesNextPageExist { get; set; }
    }
}
