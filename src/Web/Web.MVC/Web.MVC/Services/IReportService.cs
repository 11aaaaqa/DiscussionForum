namespace Web.MVC.Services
{
    public interface IReportService
    {
        Task<bool> DeleteReport(Guid reportId);
    }
}