namespace MessageBus.Messages
{
    public interface IUserNameChanged
    {
        public string OldUserName { get; set; }
        public string NewUserName { get; set; }
    }
}
