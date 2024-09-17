namespace Web.MVC.Models.ApiResponses.Discussion
{
    public class DiscussionResponse
    {
        public Guid Id { get; set; }
        public string TopicName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateOnly CreatedAt { get; set; }
        public int Rating { get; set; }
        public string CreatedBy { get; set; }
    }
}
