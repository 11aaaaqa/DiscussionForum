namespace UserMicroservice.Api.Services.User
{
    public interface IUserService<TUser> where TUser : class
    {
        Task<TUser?> GetUserByIdAsync(Guid id);
        Task<TUser?> GetUserByUserName(string userName);
        Task<List<Guid>?> GetCreatedDiscussionsIdsByUserIdAsync(Guid id);
        Task<List<Guid>?> GetSuggestedDiscussionsIdsByUserIdAsync(Guid id);
        Task<List<Guid>?> GetSuggestedCommentsIdsByUserIdAsync(Guid id);
        Task<List<Guid>?> GetCommentsIdsByUserIdAsync(Guid id);
    }
}
