using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;

namespace UserMicroservice.Api.Services.User
{
    public class UserService : IUserService<Models.User>, IChangeUserName, ICheckForNormalized
    {
        private readonly ApplicationDbContext context;

        public UserService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Models.User?> GetUserByIdAsync(Guid id) =>
            await context.Users.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<Models.User?> GetUserByUserName(string userName) =>
            await context.Users.SingleOrDefaultAsync(x => x.UserName == userName);

        public async Task<bool> ChangeUserNameAsync(Guid userId, string newUserName)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user is null) return false;
            
            user.UserName = newUserName;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsNormalizedUserNameAlreadyExists(string userName)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.UserName.ToUpper() == userName.ToUpper());
            if (user is null) return false;
            return true;
        }
    }
}
