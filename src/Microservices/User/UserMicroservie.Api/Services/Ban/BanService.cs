using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.Services.Ban
{
    public class BanService : IBanService<Models.User>
    {
        private readonly ApplicationDbContext context;

        public BanService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> IsUserBannedAsync(Guid userId)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user is null) return false;

            if (user.IsBanned)
            {
                if (DateTime.UtcNow > user.BannedUntil)
                {
                    user.IsBanned = false;
                    user.BanType = "Not banned";
                    user.BannedFor = "Not banned";
                    user.BannedUntil = new DateTime();
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
                    return false;
                }
                return true;
            }
            return false;
        }

        public async Task BanUserAsync(Guid userId, string reason, string banType, uint forDays)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user is null) return;

            user.IsBanned = true;
            user.BanType = banType;
            user.BannedFor = reason;
            user.BannedUntil = DateTime.UtcNow.AddDays(forDays);
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task BanUserAsync(string userName, string reason, string banType, uint forDays)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
            if (user is null) return;

            user.IsBanned = true;
            user.BanType = banType;
            user.BannedFor = reason;
            user.BannedUntil = DateTime.UtcNow.AddDays(forDays);
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task UnbanUserAsync(Guid userId)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user is null) return;

            user.IsBanned = false;
            user.BanType = "Not banned";
            user.BannedFor = "Not banned";
            user.BannedUntil = new DateTime();
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task UnbanUserAsync(string userName)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
            if (user is null) return;

            user.IsBanned = false;
            user.BanType = "Not banned";
            user.BannedFor = "Not banned";
            user.BannedUntil = new DateTime();
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
    }
}
