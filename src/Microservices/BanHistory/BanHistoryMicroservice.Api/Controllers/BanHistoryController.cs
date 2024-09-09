using BanHistoryMicroservice.Api.DTOs;
using BanHistoryMicroservice.Api.Models;
using BanHistoryMicroservice.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BanHistoryMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanHistoryController : ControllerBase
    {
        private readonly IBanService<Ban> banService;

        public BanHistoryController(IBanService<Ban> banService)
        {
            this.banService = banService;
        }

        [Route("GetAllBans")]
        [HttpGet]
        public async Task<IActionResult> GetAllBansAsync() => Ok(await banService.GetAllBansAsync());

        [Route("GetBansByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetBansByUserIdAsync(Guid userId) =>
            Ok(await banService.GetByUserIdAsync(userId));

        [Route("GetBansByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetBansByUserNameAsync(string userName) =>
            Ok(await banService.GetByUserNameAsync(userName));

        [Route("GetBansByBanType/{banType}")]
        [HttpGet]
        public async Task<IActionResult> GetBansByBanTypeAsync(string banType) =>
            Ok(await banService.GetByBanTypeAsync(banType));

        [Route("GetBanById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetBanByIdAsync(Guid id)
        {
            var ban = await banService.GetByIdAsync(id);
            if (ban is null) return BadRequest();

            return Ok(ban);
        }

        [Route("CreateBan")]
        [HttpPost]
        public async Task<IActionResult> CreateBanAsync(BanDto model)
        {
            var ban = await banService.CreateAsync(new Ban
            {
                Id = Guid.NewGuid(), UserName = model.UserName, Reason = model.Reason,
                DurationInDays = model.DurationInDays, BanType = model.BanType, UserId = model.UserId
            });
            return Ok(ban);
        }

        [Route("DeleteBanById/{banId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBanByIdAsync(Guid banId)
        {
            await banService.DeleteAsync(banId);
            return Ok();
        }

        [Route("DeleteBansByUserId/{userId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBansByUserIdAsync(Guid userId)
        {
            await banService.DeleteBansByUserId(userId);
            return Ok();
        }

        [Route("DeleteBansByUserName/{userName}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBansByUserNameAsync(string userName)
        {
            await banService.DeleteBansByUserName(userName);
            return Ok();
        }
    }
}
