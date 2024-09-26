namespace MessageBus.Messages
{
    public interface IUserWithUnconfirmedEmailDeleted
    {
        public Guid AspNetUserId { get; set; }
    }
}
