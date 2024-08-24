using System.Security.Claims;
using GeneralClassesLib.ApiResponses;
using MassTransit;
using MessageBus.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using RegisterMicroservice.Api.DTOs.Auth;
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
        private readonly ITokenService tokenService;
        private readonly IEmailSender emailSender;
        private readonly IPublishEndpoint publishEndpoint;

        public AuthController(UserManager<User> userManager, ITokenService tokenService, ILogger<AuthController> logger,IEmailSender emailSender,
            IPublishEndpoint publishEndpoint)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.tokenService = tokenService;
            this.emailSender = emailSender;
            this.publishEndpoint = publishEndpoint;
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
                new Claim(ClaimTypes.Name, user.UserName)
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
                Token = accessToken,
                UserName = user.UserName
            });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto model)
        {
            logger.LogInformation("Register method start working");

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = false
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                logger.LogError("Current user already exists, end method");
                return Conflict(result.Errors);
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink =
                $"{model.Uri.Protocol}://{model.Uri.DomainName}/{model.Uri.Controller}/{model.Uri.Action}?token={token}&userId={user.Id}";

            await emailSender.SendEmailAsync(new MailboxAddress("", model.Email), "Подтвердите свою почту",
                $"Подтвердите регистрацию, перейдя по <a href=\"{confirmationLink}\">ссылке</a>");

            await publishEndpoint.Publish<IUserRegistered>(new
            {
                UserId = user.Id,
                UserName = user.UserName
            });

            logger.LogInformation("Email was sent, register method ends working");

            return Ok("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
        }

        [HttpGet]
        [Route("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            logger.LogInformation("Confirm email method start working");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogError("User with current id doesn't exist, end method");
                return BadRequest();
            }

            token = token.Replace(" ", "+");

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                logger.LogCritical("Email wasn't confirmed, end method");
                return BadRequest();
            }

            logger.LogInformation("Email is successfully confirmed, end method");

            return Ok();
        }
    }
}
