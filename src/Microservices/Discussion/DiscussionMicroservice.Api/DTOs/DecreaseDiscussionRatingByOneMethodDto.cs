namespace DiscussionMicroservice.Api.DTOs
{
    public class DecreaseDiscussionRatingByOneMethodDto
    {
        public string UserNameDecreasedBy { get; set; }
        public Guid DiscussionId { get; set; }
    }
}
