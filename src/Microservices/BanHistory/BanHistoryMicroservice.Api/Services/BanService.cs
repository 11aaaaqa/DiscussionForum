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

        public async Task<List<Ban>> GetAllBansAsync() => await context.Bans.ToListAsync();

        public async Task<List<Ban>> GetByUserIdAsync(Guid userId) =>
            await context.Bans.Where(x => x.UserId == userId).ToListAsync();

        public async Task<List<Ban>> GetByUserNameAsync(string userName) =>
            await context.Bans.Where(x => x.UserName == userName).ToListAsync();

        public async Task<List<Ban>> GetByBanTypeAsync(string banType) =>
            await context.Bans.Where(x => x.BanType == banType).ToListAsync();

        public async Task<Ban?> GetByIdAsync(Guid id) => await context.Bans.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<Ban> CreateAsync(Ban model)
        {
            await context.Bans.AddAsync(model);
            await context.SaveChangesAsync();
            return model;
        }

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
