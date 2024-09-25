namespace UserMicroservice.Api.DTOs
{
    public class ChangeUserNameDto
    {
        public Guid UserId { get; set; }
        public string NewUserName { get; set; }
    }
}
