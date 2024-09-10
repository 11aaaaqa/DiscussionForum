using ReportMicroservice.Api.Models;

namespace ReportMicroservice.Api.Services
{
    public interface IReportService<TModel>
    {
        Task<List<TModel>> GetAllReportsAsync();
        Task<TModel?> GetReportByIdAsync(Guid reportId);
        Task<Report> CreateReportAsync(TModel model);
        Task DeleteReportsByUserName(string userName);
        Task<List<Report>?> GetReportsByReportType(string reportType);
        Task DeleteReportById(Guid reportId);
    }
}
