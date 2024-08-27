using Microsoft.EntityFrameworkCore;
using TopicMicroservice.Api.Models;

namespace TopicMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Topic> Topics { get; set; }
        public DbSet<SuggestedTopic> SuggestedTopics { get; set; }
    }
}
