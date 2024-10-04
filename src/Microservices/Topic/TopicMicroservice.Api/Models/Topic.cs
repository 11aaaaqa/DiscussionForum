using Microsoft.EntityFrameworkCore;

namespace TopicMicroservice.Api.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public  uint PostsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
