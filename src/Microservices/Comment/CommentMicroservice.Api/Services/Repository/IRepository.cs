namespace CommentMicroservice.Api.Services.Repository
{
    public interface IRepository<TModel>
    {
        Task<List<TModel>> GetAllAsync();
        Task<List<TModel>> GetByDiscussionIdAsync(Guid id);
        Task<TModel> GetByIdAsync(Guid id);
        Task<TModel> UpdateAsync(TModel model);
        Task<TModel> CreateAsync(TModel model);
        Task DeleteByIdAsync(Guid id);
    }
}
