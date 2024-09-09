using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.Controllers
{
    [Route("api/profile/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public UserController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [Route("GetUserByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetUserByUserNameAsync(string userName)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
            if (user is null)
                return BadRequest();
            return Ok(user);
        }
    }
}
