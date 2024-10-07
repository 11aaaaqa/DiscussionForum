namespace ReportMicroservice.Api.Services
{
    public interface IPaginationService
    {
        Task<bool> DoesNextReportsPageExistAsync(int pageSize, int pageNumber);
        Task<bool> DoesNextReportsByUserNamePageExistAsync(int pageSize, int pageNumber, string userName);
        Task<bool> DoesNextReportsByReportTypePageExistAsync(int pageSize, int pageNumber, string reportType);
    }
}
