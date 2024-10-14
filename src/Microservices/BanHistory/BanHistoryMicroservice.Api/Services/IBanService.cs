using BanHistoryMicroservice.Api.Models;

namespace BanHistoryMicroservice.Api.Services
{
    public interface IBanService<TModel>
    {
        Task<List<TModel>> GetAllBansAsync(BanHistoryParameters banHistoryParameters);
        Task<List<TModel>> FindBansAsync(string searchingString, BanHistoryParameters  banHistoryParameters);
        Task<List<TModel>> GetByUserIdAsync(Guid userId, BanHistoryParameters banHistoryParameters);
        Task<List<TModel>> GetByUserNameAsync(string userName, BanHistoryParameters banHistoryParameters);
        Task<List<TModel>> GetByBanTypeAsync(string banType, BanHistoryParameters banHistoryParameters);
        Task<TModel?> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task DeleteBansByUserId(Guid userId);
        Task DeleteBansByUserName(string userName);
    }
}
