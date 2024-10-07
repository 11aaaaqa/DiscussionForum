using Microsoft.EntityFrameworkCore;
using ReportMicroservice.Api.Database;
using ReportMicroservice.Api.Models;

namespace ReportMicroservice.Api.Services
{
    public class ReportService : IReportService<Report>, IPaginationService
    {
        private readonly ApplicationDbContext context;

        public ReportService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Report>> GetAllReportsAsync(ReportParameters reportParameters)
        {
            var reports = await context.Reports.Skip(reportParameters.PageSize * (reportParameters.PageNumber - 1))
                .Take(reportParameters.PageSize).ToListAsync();
            return reports;
        }


        public async Task<Report?> GetReportByIdAsync(Guid reportId) =>
            await context.Reports.SingleOrDefaultAsync(x => x.Id == reportId);

        public async Task<List<Report>> GetByUserNameAsync(string userName, ReportParameters reportParameters)
        {
            var reports = await context.Reports.Where(x => x.UserNameReportedBy == userName)
                .Skip(reportParameters.PageSize * (reportParameters.PageNumber - 1))
                .Take(reportParameters.PageSize)
                .ToListAsync();
            return reports;
        }

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

        public async Task<List<Report>?> GetReportsByReportType(string reportType, ReportParameters reportParameters)
        {
            var reports =  await context.Reports.Where(x => x.ReportType == reportType)
                .Skip(reportParameters.PageSize * (reportParameters.PageNumber - 1))
                .Take(reportParameters.PageSize)
                .ToListAsync();
            return reports;
        }

        public async Task DeleteReportById(Guid reportId)
        {
            var report = await context.Reports.SingleOrDefaultAsync(x => x.Id == reportId);
            if (report is null) return;
            
            context.Reports.Remove(report);
            await context.SaveChangesAsync();
        }

        public async Task<bool> DoesNextReportsPageExistAsync(int pageSize, int pageNumber)
        {
            int totalReportsCount = await context.Reports.CountAsync();
            int totalRequestedCount = pageSize * pageNumber;
            int requestedStartCount = totalRequestedCount - pageSize;
            return (totalReportsCount > requestedStartCount);
        }

        public async Task<bool> DoesNextReportsByUserNamePageExistAsync(int pageSize, int pageNumber, string userName)
        {
            int totalReportsCount = await context.Reports.Where(x => x.UserNameReportedBy == userName).CountAsync();
            int totalRequestedCount = pageSize * pageNumber;
            int requestedStartCount = totalRequestedCount - pageSize;
            return (totalReportsCount > requestedStartCount);
        }

        public async Task<bool> DoesNextReportsByReportTypePageExistAsync(int pageSize, int pageNumber, string reportType)
        {
            int totalReportsCount = await context.Reports.Where(x => x.ReportType == reportType).CountAsync();
            int totalRequestedCount = pageSize * pageNumber;
            int requestedStartCount = totalRequestedCount - pageSize;
            return (totalReportsCount > requestedStartCount);
        }
    }
}
