namespace DiscussionMicroservice.Api.DTOs
{
    public class CreateDiscussionDto
    {
        public string TopicName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedBy { get; set; }
    }
}
