using Microsoft.AspNetCore.Identity;

namespace RegisterMicroservice.Api.Models
{
    public class User : IdentityUser
    {
        public new Guid Id { get; set; }
        public uint Posts { get; set; }
        public uint Answers { get; set; }
        public DateOnly RegisteredAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
