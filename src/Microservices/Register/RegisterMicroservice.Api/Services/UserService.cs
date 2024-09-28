using MassTransit;
using MessageBus.Messages;
using Microsoft.AspNetCore.Identity;
using RegisterMicroservice.Api.Models.UserModels;

namespace RegisterMicroservice.Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IPublishEndpoint publishEndpoint;

        public UserService(UserManager<User> userManager, IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
            this.userManager = userManager;
        }
        public void DeleteUnconfirmedUser(string userId)
        {
            var user = userManager.Users.Single(x => x.Id == userId);
            userManager.DeleteAsync(user);
            publishEndpoint.Publish<IUserDeleted>(new
            {
                AspNetUserId = userId
            });
        }
    }
}
