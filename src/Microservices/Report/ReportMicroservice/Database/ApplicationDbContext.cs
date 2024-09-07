using Microsoft.EntityFrameworkCore;
using ReportMicroservice.Models;

namespace ReportMicroservice.Database
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Report> Reports { get; set; }
    }
}
