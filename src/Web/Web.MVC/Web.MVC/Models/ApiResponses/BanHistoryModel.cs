namespace Web.MVC.Models.ApiResponses
{
    public class BanHistoryModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
        public uint DurationInDays { get; set; }
        public string BanType { get; set; }
        public string BannedBy { get; set; }
        public DateTime BannedFrom { get; set; }
        public DateTime BannedUntil { get; set; }
    }
}
