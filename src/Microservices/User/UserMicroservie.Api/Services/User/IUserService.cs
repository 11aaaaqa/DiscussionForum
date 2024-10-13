namespace UserMicroservice.Api.Services.User
{
    public interface IUserService<TUser> where TUser : class
    {
        Task<TUser?> GetUserByIdAsync(Guid id);
        Task<TUser?> GetUserByUserName(string userName);
    }
}
