using GeneralClassesLib.ApiResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegisterMicroservice.Api.Models.Jwt;
using RegisterMicroservice.Api.Models.UserModels;
using RegisterMicroservice.Api.Services;

namespace RegisterMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenService tokenService;
        private readonly ILogger<TokenController> logger;

        public TokenController(UserManager<User> userManager, ITokenService tokenService, ILogger<TokenController> logger)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(TokenApiModel tokenModel)
        {
            string accessToken = tokenModel.AccessToken;
            string refreshToken = tokenModel.RefreshToken;

            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userName = principal.Identity.Name;
            
            var user = await userManager.FindByNameAsync(userName);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid request");
            }

            var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(2);
            await userManager.UpdateAsync(user);
            return Ok(new AuthenticatedResponse
            {
                RefreshToken = newRefreshToken,
                Token = newAccessToken
            });
        }

        [HttpGet]
        [Route("revoke")]
        public async Task<IActionResult> Revoke(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user is null)
            {
                return BadRequest("Invalid request");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = new DateTime();
            await userManager.UpdateAsync(user);
            return Ok();
        }
    }
}
