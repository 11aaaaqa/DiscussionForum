using BanHistoryMicroservice.Api.Models;

namespace BanHistoryMicroservice.Api.Services.Pagination
{
    public interface IPaginationService
    {
        Task<bool> DoesNextAllBansPageExistAsync(BanHistoryParameters banHistoryParameters);
        Task<bool> DoesNextBansByUserIdPageExistAsync(Guid userId, BanHistoryParameters banHistoryParameters);
        Task<bool> DoesNextBansByUserNamePageExistAsync(string userName, BanHistoryParameters banHistoryParameters);
        Task<bool> DoesNextBansByBanTypePageExistAsync(string banType, BanHistoryParameters banHistoryParameters);
        Task<bool> DoesNextFindBansPageExistAsync(string searchingString, BanHistoryParameters banHistoryParameters);
    }
}
