using BanHistoryMicroservice.Api.Database;
using BanHistoryMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BanHistoryMicroservice.Api.Services
{
    public class BanService : IBanService<Ban>
    {
        private readonly ApplicationDbContext context;

        public BanService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Ban>> GetAllBansAsync(BanHistoryParameters banHistoryParameters)
        {
            var bans =  await context.Bans
                .Skip(banHistoryParameters.PageSize * (banHistoryParameters.PageNumber - 1))
                .Take(banHistoryParameters.PageSize)
                .ToListAsync();
            return bans;
        }

        public async Task<List<Ban>> FindBansAsync(string searchingString, BanHistoryParameters banHistoryParameters)
        {
            var bans = await context.Bans.Where(x => x.UserName.ToLower().Contains(searchingString.ToLower()) |
                    x.BanType.ToLower().Contains(searchingString.ToLower()) | x.BannedBy.ToLower().Contains(searchingString.ToLower()) |
                    x.Reason.ToLower().Contains(searchingString.ToLower()))
                .Skip(banHistoryParameters.PageSize * (banHistoryParameters.PageNumber - 1))
                .Take(banHistoryParameters.PageSize)
                .ToListAsync();
            return bans;
        }

        public async Task<List<Ban>> GetByUserIdAsync(Guid userId, BanHistoryParameters banHistoryParameters)
        {
            var bans = await context.Bans.Where(x => x.UserId == userId)
                .Skip(banHistoryParameters.PageSize * (banHistoryParameters.PageNumber - 1))
                .Take(banHistoryParameters.PageSize)
                .ToListAsync();
            return bans;
        }

        public async Task<List<Ban>> GetByUserNameAsync(string userName, BanHistoryParameters banHistoryParameters)
        {
            var bans = await context.Bans.Where(x => x.UserName == userName)
                .Skip(banHistoryParameters.PageSize * (banHistoryParameters.PageNumber - 1))
                .Take(banHistoryParameters.PageSize)
                .ToListAsync();
            return bans;
        }

        public async Task<List<Ban>> GetByBanTypeAsync(string banType, BanHistoryParameters banHistoryParameters)
        {
            var bans = await context.Bans.Where(x => x.BanType == banType)
                .Skip(banHistoryParameters.PageSize * (banHistoryParameters.PageNumber - 1))
                .Take(banHistoryParameters.PageSize)
                .ToListAsync();
            return bans;
        }

        public async Task<Ban> CreateAsync(Ban model)
        {
            await context.Bans.AddAsync(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task<Ban?> GetByIdAsync(Guid id) => await context.Bans.SingleOrDefaultAsync(x => x.Id == id);

        public async Task DeleteAsync(Guid id)
        {
            var ban = await context.Bans.SingleOrDefaultAsync(x => x.Id == id);
            if (ban is null) return;

            context.Remove(ban);
            await context.SaveChangesAsync();
        }

        public async Task DeleteBansByUserId(Guid userId)
        {
            var bans = await context.Bans.Where(x => x.UserId == userId).ToListAsync();
            foreach (var ban in bans)
            {
                context.Remove(ban);
            }
            await context.SaveChangesAsync();
        }

        public async Task DeleteBansByUserName(string userName)
        {
            var bans = await context.Bans.Where(x => x.UserName == userName).ToListAsync();
            foreach (var ban in bans)
            {
                context.Remove(ban);
            }
            await context.SaveChangesAsync();
        }
    }
}
