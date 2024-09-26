using Microsoft.AspNetCore.Identity;
using RegisterMicroservice.Api.Models.UserModels;

namespace RegisterMicroservice.Api.Services
{
    public class UserDeleteService
    {
        private readonly UserManager<User> userManager;

        public UserDeleteService(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        public void DeleteUnconfirmedUser(string userId)
        {
            var user = userManager.Users.Single(x => x.Id == userId);
            userManager.DeleteAsync(user);
        }
    }
}
