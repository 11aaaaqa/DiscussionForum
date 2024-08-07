using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MimeKit;
using RegisterMicroservice.Api.Models.Jwt;
using RegisterMicroservice.Api.Models.UserModels;
using RegisterMicroservice.Api.Services;
using RegisterMicroserviceLib.DTOs.Auth;

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
        private readonly IEmailSender emailSender;

        public AuthController(UserManager<User> userManager, ITokenService tokenService, ILogger<AuthController> logger, IConfiguration configuration, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.configuration = configuration;
            this.tokenService = tokenService;
            this.emailSender = emailSender;
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
        public async Task<IActionResult> Register([FromBody] RegisterDto model, string confirmEmailMethod, string confirmEmailController)
        {
            var userExists = await userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return Conflict("Пользователь с такой почтой уже существует");
            }

            var userNameHold = await userManager.FindByNameAsync(model.UserName.ToUpper());
            if (userNameHold != null)
            {
                return Conflict("Пользователь с таким именем уже существует");
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
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
                return BadRequest("Что-то пошло не так");
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink =
                Url.Action(confirmEmailMethod, confirmEmailController, new { token, email = user.Email }, Request.Scheme);
            await emailSender.SendEmailAsync(new MailboxAddress("", model.Email), "Подтвердите свою почту",
                $"Подтвердите регистрацию, перейдя по <a href=\"{confirmationLink}\">ссылке</a>");

            return Ok("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
        }

        [HttpPost]
        [Route("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
