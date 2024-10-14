using BanHistoryMicroservice.Api.Database;
using BanHistoryMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BanHistoryMicroservice.Api.Services.Pagination
{
    public class PaginationService : IPaginationService
    {
        private readonly ApplicationDbContext context;

        public PaginationService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> DoesNextAllBansPageExistAsync(BanHistoryParameters banHistoryParameters)
        {
            int totalBansCount = await context.Bans.CountAsync();
            int totalRequestedBansCount = banHistoryParameters.PageSize * banHistoryParameters.PageNumber;
            int startedRequestedBansCount = totalRequestedBansCount - banHistoryParameters.PageSize;
            bool doesExist = (totalBansCount > startedRequestedBansCount);
            return doesExist;
        }

        public async Task<bool> DoesNextBansByUserIdPageExistAsync(Guid userId, BanHistoryParameters banHistoryParameters)
        {
            int totalBansCount = await context.Bans.Where(x => x.UserId == userId).CountAsync();
            int totalRequestedBansCount = banHistoryParameters.PageSize * banHistoryParameters.PageNumber;
            int startedRequestedBansCount = totalRequestedBansCount - banHistoryParameters.PageSize;
            bool doesExist = (totalBansCount > startedRequestedBansCount);
            return doesExist;
        }

        public async Task<bool> DoesNextBansByUserNamePageExistAsync(string userName, BanHistoryParameters banHistoryParameters)
        {
            int totalBansCount = await context.Bans.Where(x => x.UserName == userName).CountAsync();
            int totalRequestedBansCount = banHistoryParameters.PageSize * banHistoryParameters.PageNumber;
            int startedRequestedBansCount = totalRequestedBansCount - banHistoryParameters.PageSize;
            bool doesExist = (totalBansCount > startedRequestedBansCount);
            return doesExist;
        }

        public async Task<bool> DoesNextBansByBanTypePageExistAsync(string banType, BanHistoryParameters banHistoryParameters)
        {
            int totalBansCount = await context.Bans.Where(x => x.BanType == banType).CountAsync();
            int totalRequestedBansCount = banHistoryParameters.PageSize * banHistoryParameters.PageNumber;
            int startedRequestedBansCount = totalRequestedBansCount - banHistoryParameters.PageSize;
            bool doesExist = (totalBansCount > startedRequestedBansCount);
            return doesExist;
        }

        public async Task<bool> DoesNextFindBansPageExistAsync(string searchingString, BanHistoryParameters banHistoryParameters)
        {
            int totalBansCount = await context.Bans.Where(x => x.UserName.ToLower().Contains(searchingString.ToLower()) |
                  x.BanType.ToLower().Contains(searchingString.ToLower()) | x.BannedBy.ToLower().Contains(searchingString.ToLower()) |
                  x.Reason.ToLower().Contains(searchingString.ToLower())).CountAsync();
            int totalRequestedBansCount = banHistoryParameters.PageSize * banHistoryParameters.PageNumber;
            int startedRequestedBansCount = totalRequestedBansCount - banHistoryParameters.PageSize;
            bool doesExist = (totalBansCount > startedRequestedBansCount);
            return doesExist;
        }
    }
}
