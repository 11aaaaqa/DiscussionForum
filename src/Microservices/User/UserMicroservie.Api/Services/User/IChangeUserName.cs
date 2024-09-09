namespace UserMicroservice.Api.Services.User
{
    public interface IChangeUserName
    {
        Task<bool> ChangeUserNameAsync(Guid userId, string newUserName);
    }
}
