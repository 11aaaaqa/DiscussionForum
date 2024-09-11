namespace UserMicroservice.Api.Services.Ban
{
    public interface IBanService<TUser> where TUser : class
    {
        Task<bool> IsUserBannedAsync(Guid userId);
        Task<bool> IsUserBannedAsync(string userName);
        Task BanUserAsync(Guid userId, string reason, string banType, uint forDays);
        Task BanUserAsync(string userName, string reason, string banType, uint forDays);
        Task UnbanUserAsync(Guid userId);
        Task UnbanUserAsync(string userName);
    }
}
