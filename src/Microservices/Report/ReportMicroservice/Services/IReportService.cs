using ReportMicroservice.Models;

namespace ReportMicroservice.Services
{
    public interface IReportService<TModel>
    {
        Task<List<TModel>> GetAllReportsAsync();
        Task<TModel> GetReportByIdAsync(Guid reportId);
        Task<Report> CreateReportAsync(TModel model);
        Task DeleteReportsByUserId(Guid userId);
    }
}
