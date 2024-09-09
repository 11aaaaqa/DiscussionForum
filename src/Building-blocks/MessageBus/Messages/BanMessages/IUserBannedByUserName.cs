namespace MessageBus.Messages.BanMessages
{
    public interface IUserBannedByUserName
    {
        public string UserName { get; set; }
        public string Reason { get; set; }
        public string BanType { get; set; }
        public uint DurationIdDays { get; set; }
    }
}
