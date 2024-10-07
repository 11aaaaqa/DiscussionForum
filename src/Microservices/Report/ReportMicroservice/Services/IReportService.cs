using ReportMicroservice.Api.Models;

namespace ReportMicroservice.Api.Services
{
    public interface IReportService<TModel>
    {
        Task<List<TModel>> GetAllReportsAsync(ReportParameters reportParameters);
        Task<TModel?> GetReportByIdAsync(Guid reportId);
        Task<List<TModel>> GetByUserNameAsync(string userName, ReportParameters reportParameters);
        Task<TModel> CreateReportAsync(TModel model);
        Task DeleteReportsByUserName(string userName);
        Task<List<TModel>?> GetReportsByReportType(string reportType, ReportParameters reportParameters);
        Task DeleteReportById(Guid reportId);
    }
}
