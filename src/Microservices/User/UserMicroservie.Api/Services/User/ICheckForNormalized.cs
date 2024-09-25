namespace UserMicroservice.Api.Services.User
{
    public interface ICheckForNormalized
    {
        Task<bool> IsNormalizedUserNameAlreadyExists(string userName);
    }
}
