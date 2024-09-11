using Microsoft.AspNetCore.Mvc;
using UserMicroservice.Api.DTOs;
using UserMicroservice.Api.Models;
using UserMicroservice.Api.Services.Ban;
using UserMicroservice.Api.Services.User;

namespace UserMicroservice.Api.Controllers
{
    [Route("api/profile/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService<User> userService;
        private readonly IBanService<User> banService;
        private readonly IChangeUserName changeUserName;

        public UserController(IUserService<User> userService, IBanService<User> banService, IChangeUserName changeUserName)
        {
            this.banService = banService;
            this.userService = userService;
            this.changeUserName = changeUserName;
        }

        [Route("GetUserByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetUserByUserNameAsync(string userName)
        {
            var user = await userService.GetUserByUserName(userName);
            if (user is null)
                return BadRequest();
            return Ok(user);
        }

        [Route("GetUserById/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetUserByIdAsync(Guid userId)
        {
            var user = await userService.GetUserByIdAsync(userId);
            if (user is null)
                return BadRequest();
            return Ok(user);
        }

        [Route("GetCreatedDiscussionsIdsByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetCreatedDiscussionsIdsByUserIdAsync(Guid userId)
        {
            var ids = await userService.GetCreatedDiscussionsIdsByUserIdAsync(userId);
            if (ids is null) return BadRequest();

            return Ok(ids);
        }

        [Route("GetSuggestedDiscussionsIdsByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedDiscussionsIdsByUserIdAsync(Guid userId)
        {
            var ids = await userService.GetSuggestedDiscussionsIdsByUserIdAsync(userId);
            if (ids is null) return BadRequest();

            return Ok(ids);
        }

        [Route("GetSuggestedCommentsIdsByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedCommentsIdsByUserIdAsync(Guid userId)
        {
            var ids = await userService.GetSuggestedCommentsIdsByUserIdAsync(userId);
            if (ids is null) return BadRequest();

            return Ok(ids);
        }

        [Route("GetCommentsIdsByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetCommentsIdsByUserIdAsync(Guid userId)
        {
            var ids = await userService.GetCommentsIdsByUserIdAsync(userId);
            if (ids is null) return BadRequest();

            return Ok(ids);
        }

        [Route("IsUserBannedByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> IsUserBannedAsync(Guid userId) =>
            Ok(await banService.IsUserBannedAsync(userId));

        [Route("IsUserBannedByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> IsUserBannedAsync(string userName) =>
            Ok(await banService.IsUserBannedAsync(userName));

        [Route("BanUserByUserId/{userId}")]
        [HttpPost]
        public async Task<IActionResult> BanUserByIdAsync(Guid userId, [FromBody] BanDto model)
        {
            await banService.BanUserAsync(userId, model.Reason, model.BanType, model.ForDays);
            return Ok();
        }

        [Route("BanUserByUserName/{userName}")]
        [HttpPost]
        public async Task<IActionResult> BanUserByUserNameAsync(string userName, [FromBody] BanDto model)
        {
            await banService.BanUserAsync(userName, model.Reason, model.BanType, model.ForDays);
            return Ok();
        }

        [Route("UnbanUserByUserId/{userId}")]
        [HttpPost]
        public async Task<IActionResult> UnbanUserByUserIdAsync(Guid userId)
        {
            await banService.UnbanUserAsync(userId);
            return Ok();
        }

        [Route("UnbanUserByUserName/{userName}")]
        [HttpPost]
        public async Task<IActionResult> UnbanUserByUserNameAsync(string userName)
        {
            await banService.UnbanUserAsync(userName);
            return Ok();
        }

        [Route("ChangeUserName/{userId}")]
        [HttpPost]
        public async Task<IActionResult> ChangeUserNameAsync(Guid userId, [FromBody] string newUserName)
        {
            var isChanged = await changeUserName.ChangeUserNameAsync(userId, newUserName);
            if (isChanged)
                return Ok();
            return BadRequest();
        }
    }
}
