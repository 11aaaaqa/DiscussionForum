using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GeneralClassesLib.ApiResponses;
using Microsoft.AspNetCore.Identity;
using RegisterMicroservice.Api.Constants;
using RegisterMicroservice.Api.DTOs.Auth;
using RegisterMicroservice.Api.Models.UserModels;

namespace RegisterMicroservice.IntegrationTests
{
    [CollectionDefinition("UserManagerCollection")]
    public class UserManagerCollection : ICollectionFixture<UserManager<User>>{}

    [Collection("UserManagerCollection")]
    public class ControllersTests : IClassFixture<TestingWebAppFactory>
    {
        private readonly HttpClient httpClient;

        public ControllersTests(TestingWebAppFactory factory, UserManager<User> userManager)
        {
            httpClient = factory.CreateClient();

            var user1 = new User { UserName = "confirmedUser", Id = Guid.NewGuid().ToString(), Email = "user@gmail.com", EmailConfirmed = true };
            userManager.CreateAsync(user1, "testPassword");
            userManager.AddToRoleAsync(user1, UserRoleConstants.UserRole);

            var user2 = new User { UserName = "admin", Id = Guid.NewGuid().ToString(), Email = "admin@gmail.com", EmailConfirmed = true };
            userManager.CreateAsync(user2, "testPassword");
            userManager.AddToRoleAsync(user2, UserRoleConstants.UserRole);
            userManager.AddToRoleAsync(user2, UserRoleConstants.AdminRole);

            var user3 = new User { UserName = "moderator", Id = Guid.NewGuid().ToString(), Email = "moderator@gmail.com", EmailConfirmed = true };
            userManager.CreateAsync(user3, "testPassword");
            userManager.AddToRoleAsync(user3, UserRoleConstants.UserRole);
            userManager.AddToRoleAsync(user3, UserRoleConstants.ModeratorRole);

            var user4 = new User { UserName = "unconfirmedUser", Id = Guid.NewGuid().ToString(), Email = "unconfirmedUser@gmail.com", EmailConfirmed = false };
            userManager.CreateAsync(user4, "testPassword");
            userManager.AddToRoleAsync(user4, UserRoleConstants.UserRole);

            var user5 = new User { UserName = "confirmedUser2", Id = Guid.NewGuid().ToString(), Email = "user2@gmail.com", EmailConfirmed = true };
            userManager.CreateAsync(user5, "testPassword");
            userManager.AddToRoleAsync(user5, UserRoleConstants.UserRole);
        }

        [Fact]
        public async Task Login_ReturnsOk()
        {
            var model = new LoginDto { Password = "testPassword", UserNameOrEmail = "confirmedUser" };
            using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"api/Auth/login", jsonContent);

            response.EnsureSuccessStatusCode();
            var responseModel = await response.Content.ReadFromJsonAsync<AuthenticatedResponse>();
            Assert.NotNull(responseModel);
            Assert.NotEmpty(responseModel.Token);
        }
    }
}
