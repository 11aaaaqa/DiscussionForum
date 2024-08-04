using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using RegisterMicroservice.Api.DTOs;
using RegisterMicroservice.Api.Models.Jwt;
using RegisterMicroservice.Api.Models.Response;
using RegisterMicroservice.Api.Models.UserModels;
using RegisterMicroservice.Api.Services;

namespace RegisterMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<AuthController> logger;
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;

        public AuthController(UserManager<User> userManager, ITokenService tokenService, ILogger<AuthController> logger, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.configuration = configuration;
            this.tokenService = tokenService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto model)
        {
            var emailUser = await userManager.FindByEmailAsync(model.UserNameOrEmail);
            var userNameUser = await userManager.FindByNameAsync(model.UserNameOrEmail.ToUpper());
            var user = emailUser ?? userNameUser;

            if (user == null || !await userManager.CheckPasswordAsync(user,model.Password))
            {
                return Unauthorized();
            }

            var userRoles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var accessToken = tokenService.GenerateAccessToken(claims);
            var refreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.MaxValue;

            await userManager.UpdateAsync(user);

            return Ok(new AuthenticatedResponse
            {
                RefreshToken = refreshToken,
                Token = accessToken
            });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var userExists = await userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return Conflict(new RegisterResponseModel
                { Message = "User with current email already exists", Status = "Error" });
            }

            var userNameHold = await userManager.FindByNameAsync(model.UserName.ToUpper());
            if (userNameHold != null)
            {
                return Conflict(new RegisterResponseModel { Message = "User with current username already exists" });
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Posts = 0,
                Answers = 0,
                RegisteredAt = DateOnly.FromDateTime(DateTime.UtcNow),
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = false
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new RegisterResponseModel { Message = "Something went wrong", Status = "Error" });
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink =
                Url.Action(nameof(ConfirmEmail), "Auth", new { token, email = user.Email }, Request.Scheme);

        }
    }
}
