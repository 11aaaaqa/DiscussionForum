using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Models;

namespace UserMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
