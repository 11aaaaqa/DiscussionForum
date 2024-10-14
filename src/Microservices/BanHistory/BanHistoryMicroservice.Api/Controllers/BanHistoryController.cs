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
        public async Task<IActionResult> GetAllBansAsync([FromQuery] BanHistoryParameters banHistoryParameters)
            => Ok(await banService.GetAllBansAsync(banHistoryParameters));

        [Route("GetBansByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetBansByUserIdAsync(Guid userId, [FromQuery] BanHistoryParameters banHistoryParameters) =>
            Ok(await banService.GetByUserIdAsync(userId, banHistoryParameters));

        [Route("GetBansByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetBansByUserNameAsync(string userName, [FromQuery] BanHistoryParameters banHistoryParameters) =>
            Ok(await banService.GetByUserNameAsync(userName, banHistoryParameters));

        [Route("GetBansByBanType/{banType}")]
        [HttpGet]
        public async Task<IActionResult> GetBansByBanTypeAsync(string banType, [FromQuery] BanHistoryParameters banHistoryParameters) =>
            Ok(await banService.GetByBanTypeAsync(banType, banHistoryParameters));

        [Route("GetBanById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetBanByIdAsync(Guid id)
        {
            var ban = await banService.GetByIdAsync(id);
            if (ban is null) return BadRequest();

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
