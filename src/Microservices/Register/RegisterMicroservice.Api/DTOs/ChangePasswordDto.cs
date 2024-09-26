namespace RegisterMicroservice.Api.DTOs
{
    public class ChangePasswordDto
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
