namespace RegisterMicroservice.Api.DTOs.User
{
    public class AddUserToRoleDto
    {
        public string UserId { get; set; }
        public List<string> RoleNames { get; set; }
    }
}
