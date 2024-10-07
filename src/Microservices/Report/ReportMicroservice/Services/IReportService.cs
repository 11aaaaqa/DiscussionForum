using ReportMicroservice.Api.Models;

namespace ReportMicroservice.Api.Services
{
    public interface IReportService<TModel>
    {
        Task<List<TModel>> GetAllReportsAsync(ReportParameters reportParameters);
        Task<TModel?> GetReportByIdAsync(Guid reportId);
        Task<List<TModel>> GetByUserNameAsync(string userName);
        Task<Report> CreateReportAsync(TModel model);
        Task DeleteReportsByUserName(string userName);
        Task<List<Report>?> GetReportsByReportType(string reportType);
        Task DeleteReportById(Guid reportId);
    }
}
