using Microsoft.AspNetCore.Identity;

namespace RegisterMicroservice.Api.Models.UserModels
{
    public class User : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string? HangfireDelayedJobId { get; set; }
    }
}
