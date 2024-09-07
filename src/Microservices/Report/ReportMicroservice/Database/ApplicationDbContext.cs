using Microsoft.EntityFrameworkCore;
using ReportMicroservice.Api.Models;

namespace ReportMicroservice.Api.Database
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Report> Reports { get; set; }
    }
}
