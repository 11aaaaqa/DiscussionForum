namespace DiscussionMicroservice.Api.DTOs
{
    public class IncreaseDiscussionRatingByOneDto
    {
        public string UserNameIncreasedBy { get; set; }
        public Guid DiscussionId { get; set; }
    }
}
