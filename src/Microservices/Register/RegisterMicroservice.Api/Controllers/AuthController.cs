using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using RegisterMicroservice.Api.DTOs;
using RegisterMicroservice.Api.Models.TokenModels;
using RegisterMicroservice.Api.Models.UserModels;

namespace RegisterMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<AuthController> logger;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;

        public AuthController(UserManager<User> userManager, ILogger<AuthController> logger, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var checkUser = await userManager.FindByEmailAsync(model.Email);
            if (checkUser != null)
            {
                logger.LogInformation("User wasn't created due to \"User already exists\"");
                return BadRequest(new Response{Message = "User already exists", Status = "Error"});
            }

            var user = new User
            {
                Answers = 0,
                Id = Guid.NewGuid(),
                Email = model.Email,
                EmailConfirmed = false,
                Posts = 0,
                RegisteredAt = DateOnly.FromDateTime(DateTime.Now),
                UserName = model.UserName
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                logger.LogInformation("User wasn't created due to \"Something went wrong\"");
                return BadRequest(new Response { Status = "Error", Message = "Something went wrong" });
            }
                
            logger.LogInformation("New user was created");
            return Ok(new Response { Status = "Success", Message = "User was successfully created" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            return Ok();
        }
    }
}
