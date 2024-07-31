using System.Security.Claims;

namespace RegisterMicroservice.Api.Services
{
    public interface ITokenService
    {
        string GenerateSuccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
