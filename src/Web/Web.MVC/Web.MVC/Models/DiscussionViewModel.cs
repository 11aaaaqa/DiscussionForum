using Web.MVC.Models.ApiResponses.Discussion;

namespace Web.MVC.Models
{
    public class DiscussionViewModel
    {
        public List<DiscussionResponse> Discussions { get; set; }

        public string TopicName { get; set; }
    }
}
