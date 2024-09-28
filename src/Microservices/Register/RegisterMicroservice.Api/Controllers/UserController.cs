using GeneralClassesLib.ApiResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using RegisterMicroservice.Api.DTOs.Auth;
using RegisterMicroservice.Api.Models.UserModels;
using RegisterMicroservice.Api.Services;
using System.Security.Claims;
using MassTransit;
using MessageBus.Messages;
using Microsoft.EntityFrameworkCore;
using RegisterMicroservice.Api.Constants;
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
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IPublishEndpoint publishEndpoint;

        public UserController(UserManager<User> userManager, ILogger<AuthController> logger, IEmailSender emailSender, ITokenService tokenService,
            RoleManager<IdentityRole> roleManager, IPublishEndpoint publishEndpoint)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.tokenService = tokenService;
            this.roleManager = roleManager;
            this.publishEndpoint = publishEndpoint;
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

        [Route("GetUserRolesByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetUserRolesByUserIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();
            var roles = await userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        [Route("AddUserToRoles")]
        [HttpPost]
        public async Task<IActionResult> AddUserToRolesAsync([FromBody]AddUserToRoleDto model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user is null) return NotFound();

            if (model.RoleNames.Count == 0)
            {
                var allRoles = await roleManager.Roles.ToListAsync();
                foreach (var role in allRoles)
                {
                    if(role.Name != UserRoleConstants.UserRole)
                        await userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }
            else
            {
                var allRoles = await roleManager.Roles.ToListAsync();
                var userRole = allRoles.Single(x => x.Name == UserRoleConstants.UserRole);
                allRoles.Remove(userRole);
                foreach (var role in allRoles)
                {
                    if (model.RoleNames.Contains(role.Name) && !await userManager.IsInRoleAsync(user, role.Name))
                        await userManager.AddToRoleAsync(user, role.Name);
                    else if (model.RoleNames.Contains(role.Name) && await userManager.IsInRoleAsync(user, role.Name))
                        continue;
                    else
                    {
                        await userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                }
            }
            
            return Ok();
        }
    }
}
