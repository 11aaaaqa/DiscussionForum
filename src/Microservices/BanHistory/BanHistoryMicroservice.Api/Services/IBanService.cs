namespace BanHistoryMicroservice.Api.Services
{
    public interface IBanService<TModel>
    {
        Task<List<TModel>> GetAllBansAsync();
        Task<List<TModel>> GetByUserIdAsync(Guid userId);
        Task<List<TModel>> GetByUserNameAsync(string userName);
        Task<List<TModel>> GetByBanTypeAsync(string banType);
        Task<TModel?> GetByIdAsync(Guid id);
        Task<TModel> CreateAsync(TModel model);
        Task DeleteAsync(Guid id);
        Task DeleteBansByUserId(Guid userId);
        Task DeleteBansByUserName(string userName);
    }
}
