using BanHistoryMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BanHistoryMicroservice.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Ban> Bans { get; set; }
    }
}
