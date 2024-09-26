namespace MessageBus.Messages
{
    public interface IUserWithUnconfirmedEmailDeleted
    {
        public string AspNetUserId { get; set; }
    }
}
