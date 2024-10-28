using Web.MVC.Models.ApiResponses.CustUserResponses;

namespace Web.MVC.Services
{
    public class CheckUserService : ICheckUserService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;
        
        public CheckUserService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            url = (string.IsNullOrEmpty(config["Url:Port"]))
                ? $"{config["Url:Protocol"]}://{config["Url:HostName"]}" : $"{config["Url:Protocol"]}://{config["Url:HostName"]}:{config["Url:Port"]}";
        }
        public async Task<bool> HasUserCreatedSpecifiedDiscussionsCount(string userName, uint discussionsCount)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"{url}/api/profile/User/GetUserByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return false;

            var user = await response.Content.ReadFromJsonAsync<UserResponse>();
            if (user.Posts >= discussionsCount) return true;
            return false;
        }

        public async Task<bool> HasUserCreatedSpecifiedCommentsCount(string userName, uint commentsCount)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"{url}/api/profile/User/GetUserByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return false;

            var user = await response.Content.ReadFromJsonAsync<UserResponse>();
            if (user.Answers >= commentsCount) return true;
            return false;
        }
    }
}
