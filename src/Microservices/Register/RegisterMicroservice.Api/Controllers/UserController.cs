using GeneralClassesLib.ApiResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using RegisterMicroservice.Api.DTOs.Auth;
using RegisterMicroservice.Api.Models.UserModels;
using RegisterMicroservice.Api.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RegisterMicroservice.Api.DTOs;
using RegisterMicroservice.Api.DTOs.User;

namespace RegisterMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<AuthController> logger;
        private readonly IEmailSender emailSender;
        private readonly ITokenService tokenService;

        public UserController(UserManager<User> userManager, ILogger<AuthController> logger, IEmailSender emailSender, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.tokenService = tokenService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetUserByIdAsync(string uid)
        {
            var user = await userManager.FindByIdAsync(uid);
            if (user == null)
            {
                return BadRequest("Пользователя с таким идентификатором не существует");
            }

            return Ok(user);
        }

        [HttpGet("GetByUserName")]
        public async Task<IActionResult> GetUserByUserNameAsync(string userName)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
                return BadRequest("Пользователя с таким именем не существует");

            return Ok(user);
        }

        [HttpGet("GetByUserNameOrEmail")]
        public async Task<IActionResult> GetUserByUserNameOrEmailAsync(string userNameOrEmail)
        {
            var userNameUser = await userManager.Users.SingleOrDefaultAsync(x => x.UserName == userNameOrEmail);
            var emailUser = await userManager.Users.SingleOrDefaultAsync(x => x.Email == userNameOrEmail);
            var user = userNameUser ?? emailUser;
            if (user == null)
                return BadRequest("Пользователя с таким именем не существует");
            
            return Ok(user);
        }

        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetUserByEmailAsync(string email)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(x => x.Email == email);
            if (user == null)
                return BadRequest("Пользователя с такой почтой не существует");

            return Ok(user);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUserById(string uid)
        {
            var result = await userManager.DeleteAsync(new User { Id = uid });
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest("Пользователя с таким именем не существует");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody]User user)
        {
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest();
            

            return Ok(user);
        }

        [HttpPost("SendEmailConfirmationLink")]
        public async Task<IActionResult> SendEmailConfirmationLink([FromBody] ConfirmEmailMethodUri uri, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("Такого пользователя не существует");
            }
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink =
                $"{uri.Protocol}://{uri.DomainName}/{uri.Controller}/{uri.Action}?token={token}&userId={user.Id}";

            await emailSender.SendEmailAsync(new MailboxAddress("", user.Email), "Подтвердите свою почту",
                $"Подтвердите регистрацию, перейдя по <a href=\"{confirmationLink}\">ссылке</a>");
            return Ok();
        }

        [Route("AuthUserByUserName")]
        [HttpGet]
        public async Task<IActionResult> AuthUserByUserName(string userName)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(x => x.UserName == userName);

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
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(2);

            await userManager.UpdateAsync(user);

            return Ok(new AuthenticatedResponse
            {
                Token = accessToken
            });
        }

        [Route("ChangePassword")]
        [HttpPatch]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto model)
        {
            var user = await userManager.FindByIdAsync(model.UserId.ToString());
            if (user is null) return NotFound();
            
            var isPasswordChanged = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!isPasswordChanged.Succeeded) return BadRequest();

            return Ok();
        }

        [Route("AddUserToRole")]
        [HttpPost]
        public async Task<IActionResult> AddUserToRoleAsync([FromBody]AddUserToRoleDto model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user is null) return NotFound();

            var result = await userManager.AddToRoleAsync(user, model.RoleName);
            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }
    }
}
