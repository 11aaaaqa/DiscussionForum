namespace RegisterMicroservice.Api.Services
{
    public interface IUserService
    {
        void DeleteUnconfirmedUser(string userId);
    }
}
