using Web.MVC.Models.ApiResponses;

namespace Web.MVC.Models
{
    public class GetSuggestedTopicViewModel
    {
        public List<TopicResponse> SuggestedTopics { get; set; }
        public bool DoesNextPageExist { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NextPageNumber { get; set; }
        public int PreviousPageNumber { get; set;}
    }
}
