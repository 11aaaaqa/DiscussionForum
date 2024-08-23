namespace MessageBus.Messages
{
    public interface IUserRegistered
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
