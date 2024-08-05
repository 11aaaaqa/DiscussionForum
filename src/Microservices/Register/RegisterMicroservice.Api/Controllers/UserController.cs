using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using RegisterMicroservice.Api.DTOs.ResetPassword;
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
        private readonly IConfiguration configuration;
        private readonly IEmailSender emailSender;

        public UserController(UserManager<User> userManager, ILogger<AuthController> logger, IConfiguration configuration, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.configuration = configuration;
            this.emailSender = emailSender;
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordDto model, string callbackController, string callbackMethod)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("User with current email doesn't exist");
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest("Email doesn't confirmed");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(callbackMethod, callbackController, new {userId = user.Id, token = token},
                protocol: HttpContext.Request.Scheme);
            await emailSender.SendEmailAsync(new MailboxAddress("", model.Email), "Сброс пароля",
                $"Для сброса пароля перейдите по <a href=\"{callbackUrl}\">ссылке</a>");

            return Content("Для сброса пароля проверьте электронную почту и перейдите по ссылке, указанной в письме");
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
