namespace TopicMicroservice.Api.Services.Repository
{
    public interface IRepository<TModel>
    {
        Task<List<TModel>> GetAllAsync();
        Task<TModel?> GetByIdAsync(Guid id);
        Task<TModel?> GetByNameAsync(string name);
        Task<TModel> UpdateAsync(TModel model);
        Task<TModel> CreateAsync(TModel model);
        Task DeleteByNameAsync(string name);
        Task DeleteByIdAsync(Guid id);
    }
}
