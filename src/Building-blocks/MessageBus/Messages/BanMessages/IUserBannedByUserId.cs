namespace MessageBus.Messages.BanMessages
{
    public interface IUserBannedByUserId
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; }
        public string BanType { get; set; }
        public uint DurationIdDays { get; set; }
    }
}
