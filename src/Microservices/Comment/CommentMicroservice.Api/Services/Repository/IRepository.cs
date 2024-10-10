using CommentMicroservice.Api.Models;

namespace CommentMicroservice.Api.Services.Repository
{
    public interface IRepository<TModel>
    {
        Task<List<TModel>> GetAllAsync(CommentParameters commentParameters);
        Task<List<TModel>> GetByDiscussionIdAsync(Guid id, CommentParameters commentParameters);
        Task<List<TModel>> GetByIds(params Guid[] ids);
        Task<List<TModel>> GetByUserName(string userName);
        Task<TModel?> GetByIdAsync(Guid id);
        Task<TModel> UpdateAsync(TModel model);
        Task<TModel> CreateAsync(TModel model);
        Task DeleteByIdAsync(Guid id);
        Task DeleteByUserNameAsync(string userName);
    }
}
