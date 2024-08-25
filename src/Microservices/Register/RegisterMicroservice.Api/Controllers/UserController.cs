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
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<AuthController> logger;
        private readonly IEmailSender emailSender;

        public UserController(UserManager<User> userManager, ILogger<AuthController> logger, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.emailSender = emailSender;
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
            var user = await userManager.FindByNameAsync(userName.ToUpper());
            if (user == null)
            {
                return BadRequest("Пользователя с таким именем не существует");
            }

            return Ok(user);
        }

        [HttpGet("GetByUserNameOrEmail")]
        public async Task<IActionResult> GetUserByUserNameOrEmailAsync(string userNameOrEmail)
        {
            var userNameUser = await userManager.FindByNameAsync(userNameOrEmail.ToUpper());
            var emailUser = await userManager.FindByEmailAsync(userNameOrEmail);
            var user = userNameUser ?? emailUser;
            if (user == null)
            {
                return BadRequest("Пользователя с таким именем не существует");
            }

            return Ok(user);
        }

        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetUserByEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Пользователя с такой почтой не существует");
            }

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
            if (result.Succeeded)
            {
                return Ok(user);
            }

            return BadRequest("Такого пользователя не существует");
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
    }
}
