using BanHistoryMicroservice.Api.Models;
using BanHistoryMicroservice.Api.Services;
using BanHistoryMicroservice.Api.Services.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace BanHistoryMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanHistoryController : ControllerBase
    {
        private readonly IBanService<Ban> banService;
        private readonly IPaginationService paginationService;

        public BanHistoryController(IBanService<Ban> banService, IPaginationService paginationService)
        {
            this.banService = banService;
            this.paginationService = paginationService;
        }

        [Route("GetAllBans")]
        [HttpGet]
        public async Task<IActionResult> GetAllBansAsync([FromQuery] BanHistoryParameters banHistoryParameters)
            => Ok(await banService.GetAllBansAsync(banHistoryParameters));

        [Route("DoesNextAllBansPageExist")]
        [HttpGet]
        public async Task<IActionResult> DoesNextAllBansPageExistAsync([FromQuery] BanHistoryParameters banHistoryParameters)
            => Ok(await paginationService.DoesNextAllBansPageExistAsync(banHistoryParameters));

        [Route("GetBansByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetBansByUserIdAsync(Guid userId, [FromQuery] BanHistoryParameters banHistoryParameters) =>
            Ok(await banService.GetByUserIdAsync(userId, banHistoryParameters));

        [Route("DoesNextBansByUserIdPageExist/{userId}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextBansByUserIdPageExistAsync(Guid userId, [FromQuery] BanHistoryParameters banHistoryParameters)
            => Ok(await paginationService.DoesNextBansByUserIdPageExistAsync(userId, banHistoryParameters));

        [Route("GetBansByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetBansByUserNameAsync(string userName, [FromQuery] BanHistoryParameters banHistoryParameters) =>
            Ok(await banService.GetByUserNameAsync(userName, banHistoryParameters));

        [Route("DoesNextBansByUserNamePageExist/{userName}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextBansByUserNamePageExistAsync(string userName,[FromQuery] BanHistoryParameters banHistoryParameters)
            => Ok(await paginationService.DoesNextBansByUserNamePageExistAsync(userName, banHistoryParameters));

        [Route("GetBansByBanType/{banType}")]
        [HttpGet]
        public async Task<IActionResult> GetBansByBanTypeAsync(string banType, [FromQuery] BanHistoryParameters banHistoryParameters) =>
            Ok(await banService.GetByBanTypeAsync(banType, banHistoryParameters));

        [Route("DoesNextBansByBanTypePageExist/{banType}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextBansByBanTypePageExistAsync(string banType, [FromQuery] BanHistoryParameters banHistoryParameters)
            => Ok(await paginationService.DoesNextBansByBanTypePageExistAsync(banType, banHistoryParameters));

        [Route("FindBans")]
        [HttpGet]
        public async Task<IActionResult> FindBansAsync([FromQuery] string searchingString, [FromQuery] BanHistoryParameters banHistoryParameters)
        {
            var bans = await banService.FindBansAsync(searchingString, banHistoryParameters);
            return Ok(bans);
        }

        [Route("DoesNextFindBansPageExist")]
        [HttpGet]
        public async Task<IActionResult> DoesNextFindBansPageExistAsync([FromQuery]string searchingString,
            [FromQuery] BanHistoryParameters banHistoryParameters)
            => Ok(await paginationService.DoesNextFindBansPageExistAsync(searchingString, banHistoryParameters));

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
