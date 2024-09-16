namespace Web.MVC.Models.ApiResponses.CustUserResponses
{
    public class UserBanInfoResponse
    {
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? BanType { get; set; }
        public string? BanReason { get; set; }
        public bool IsBanned { get; set; }
        public DateTime BannedUntil { get; set; }
    }
}
