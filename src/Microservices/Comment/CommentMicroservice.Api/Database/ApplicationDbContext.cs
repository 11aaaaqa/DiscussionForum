using CommentMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<SuggestedComment> SuggestedComments { get; set; }
    }
}
