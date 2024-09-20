using Microsoft.EntityFrameworkCore;
using ReportMicroservice.Api.Database;
using ReportMicroservice.Api.Models;

namespace ReportMicroservice.Api.Services
{
    public class ReportService : IReportService<Report>
    {
        private readonly ApplicationDbContext context;

        public ReportService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Report>> GetAllReportsAsync() => await context.Reports.ToListAsync();


        public async Task<Report?> GetReportByIdAsync(Guid reportId) =>
            await context.Reports.SingleOrDefaultAsync(x => x.Id == reportId);

        public async Task<List<Report>> GetByUserNameAsync(string userName)
            => await context.Reports.Where(x => x.UserNameReportedBy == userName).ToListAsync();

        public async Task<Report> CreateReportAsync(Report model)
        {
            await context.Reports.AddAsync(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task DeleteReportsByUserName(string userName)
        {
            var reports = await context.Reports.Where(x => x.UserNameReportedBy == userName).ToListAsync();
            foreach (var report in reports)
            {
                context.Reports.Remove(report);
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<Report>?> GetReportsByReportType(string reportType) =>
            await context.Reports.Where(x => x.ReportType == reportType).ToListAsync();

        public async Task DeleteReportById(Guid reportId)
        {
            var report = await context.Reports.SingleOrDefaultAsync(x => x.Id == reportId);
            if (report is null) return;
            
            context.Reports.Remove(report);
            await context.SaveChangesAsync();
        }
    }
}
