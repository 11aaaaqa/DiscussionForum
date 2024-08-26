using DiscussionMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<Discussion> SuggestedDiscussions { get; set; }
    }
}
