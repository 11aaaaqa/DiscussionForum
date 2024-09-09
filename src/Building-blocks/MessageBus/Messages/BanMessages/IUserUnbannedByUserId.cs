namespace MessageBus.Messages.BanMessages
{
    public interface IUserUnbannedByUserId
    {
        public Guid UserId { get; set; }
    }
}
