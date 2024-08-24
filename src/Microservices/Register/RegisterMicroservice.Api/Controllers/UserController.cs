using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegisterMicroservice.Api.Models.UserModels;

namespace RegisterMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<AuthController> logger;

        public UserController(UserManager<User> userManager, ILogger<AuthController> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
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
        public async Task<IActionResult> DeleteUserByUserName(string userName)
        {
            var result = await userManager.DeleteAsync(new User { UserName = userName });
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
    }
}
