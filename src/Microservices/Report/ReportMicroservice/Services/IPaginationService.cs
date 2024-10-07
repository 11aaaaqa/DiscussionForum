namespace ReportMicroservice.Api.Services
{
    public interface IPaginationService
    {
        Task<bool> DoesNextReportsPageExistAsync(int pageSize, int pageNumber);
    }
}
