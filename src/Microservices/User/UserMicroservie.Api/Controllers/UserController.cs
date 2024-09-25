using MassTransit;
using MessageBus.Messages;
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
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ICheckForNormalized checkForNormalized;

        public UserController(IUserService<User> userService, IBanService<User> banService, IChangeUserName changeUserName,
            IPublishEndpoint publishEndpoint, ICheckForNormalized checkForNormalized)
        {
            this.banService = banService;
            this.userService = userService;
            this.changeUserName = changeUserName;
            this.publishEndpoint = publishEndpoint;
            this.checkForNormalized = checkForNormalized;
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

        [Route("IsUserBannedByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> IsUserBannedAsync(string userName, [FromQuery(Name = "banTypes[]")]params string[] banTypes) =>
            Ok(await banService.IsUserBannedAsync(userName, banTypes));

        [Route("IsUserBannedByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> IsUserBannedAsync(Guid userId, [FromQuery(Name = "banTypes[]")] params string[] banTypes) =>
            Ok(await banService.IsUserBannedAsync(userId, banTypes));

        [Route("BanUserByUserId/{userId}")]
        [HttpPatch]
        public async Task<IActionResult> BanUserByIdAsync(Guid userId, [FromBody] BanDto model)
        {
            await banService.BanUserAsync(userId, model.Reason, model.BanType, model.ForDays);
            return Ok();
        }

        [Route("BanUserByUserName/{userName}")]
        [HttpPatch]
        public async Task<IActionResult> BanUserByUserNameAsync(string userName, [FromBody] BanDto model)
        {
            await banService.BanUserAsync(userName, model.Reason, model.BanType, model.ForDays);
            return Ok();
        }

        [Route("UnbanUserByUserId/{userId}")]
        [HttpPatch]
        public async Task<IActionResult> UnbanUserByUserIdAsync(Guid userId)
        {
            await banService.UnbanUserAsync(userId);
            return Ok();
        }

        [Route("UnbanUserByUserName/{userName}")]
        [HttpPatch]
        public async Task<IActionResult> UnbanUserByUserNameAsync(string userName)
        {
            await banService.UnbanUserAsync(userName);
            return Ok();
        }

        [Route("ChangeUserName")]
        [HttpPatch]
        public async Task<IActionResult> ChangeUserNameAsync([FromBody] ChangeUserNameDto model)
        {
            var user = await userService.GetUserByIdAsync(model.UserId);
            if (user == null) return BadRequest();

            string oldUserName = user.UserName;
            var isChanged = await changeUserName.ChangeUserNameAsync(model.UserId, model.NewUserName);
            if (isChanged)
            {
                await publishEndpoint.Publish<IUserNameChanged>(new
                {
                    OldUserName = oldUserName, model.NewUserName
                });
                return Ok();
            }
            return BadRequest();
        }

        [Route("UserBanInfoByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> UserBanInfoByUserNameAsync(string userName)
        {
            var isBanned = await banService.IsUserBannedAsync(userName);
            if (!isBanned) return Ok(new UserBanInfoModel{BannedUntil = new DateTime(), IsBanned = false, UserName = userName});

            var user = await userService.GetUserByUserName(userName);
            if (user is null) return BadRequest();

            return Ok(new UserBanInfoModel
            {
                BanReason = user.BannedFor, UserName = userName, BanType = user.BanType, BannedUntil = user.BannedUntil, IsBanned = true
            });
        }

        [Route("UserBanInfoByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> UserBanInfoByUserIdAsync(Guid userId)
        {
            var isBanned = await banService.IsUserBannedAsync(userId);
            if (!isBanned) return Ok(new UserBanInfoModel { BannedUntil = new DateTime(), IsBanned = false, UserId = userId });

            var user = await userService.GetUserByIdAsync(userId);
            if (user is null) return BadRequest();

            return Ok(new UserBanInfoModel
            {
                BanReason = user.BannedFor,
                UserId = userId,
                BanType = user.BanType,
                BannedUntil = user.BannedUntil,
                IsBanned = true
            });
        }

        [Route("IsNormalizedUserNameAlreadyExists/{userName}")]
        [HttpGet]
        public async Task<IActionResult> IsNormalizedUserNameAlreadyExistsAsync(string userName)
        {
            var isExist = await checkForNormalized.IsNormalizedUserNameAlreadyExists(userName);
            return Ok(isExist);
        }
    }
}
