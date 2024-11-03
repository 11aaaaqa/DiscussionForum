using Web.MVC.Models.ApiResponses;

namespace Web.MVC.Models.ViewModels.ModeratorViewModels
{
    public class SuggestedTopicsByUserNameViewModel
    {
        public List<TopicResponse> SuggestedTopics { get; set; }
        public bool DoesNextPageExist { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NextPageNumber { get; set; }
        public int PreviousPageNumber { get; set; }
        public string SuggestedByUserName { get; set; }
    }
}
