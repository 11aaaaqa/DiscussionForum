using Microsoft.AspNetCore.Identity;

namespace RegisterMicroservice.Api.Models.UserModels
{
    public class User : IdentityUser
    {
        public uint Posts { get; set; }
        public uint Answers { get; set; }
        public DateOnly RegisteredAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
