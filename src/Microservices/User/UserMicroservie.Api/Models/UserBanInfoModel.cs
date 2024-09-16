namespace UserMicroservice.Api.Models
{
    public class UserBanInfoModel
    {
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? BanType { get; set; }
        public string? BanReason { get; set; }
        public bool IsBanned { get; set; }
        public DateTime BannedUntil { get; set; }
    }
}
