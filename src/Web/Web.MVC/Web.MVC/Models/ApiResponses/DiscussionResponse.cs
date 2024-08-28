namespace Web.MVC.Models.ApiResponses
{
    public class DiscussionResponse
    {
        public Guid Id { get; set; }
        public string TopicName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; }
        public string CreatedBy { get; set; }
    }
}
