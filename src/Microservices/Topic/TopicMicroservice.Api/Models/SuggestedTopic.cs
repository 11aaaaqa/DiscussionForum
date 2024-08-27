using Microsoft.EntityFrameworkCore;

namespace TopicMicroservice.Api.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class SuggestedTopic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public uint PostsCount { get; set; }
    }
}
