using Web.MVC.Models.ApiResponses.CustUserResponses;

namespace Web.MVC.Services
{
    public class CheckUserService : ICheckUserService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CheckUserService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<bool> HasUserCreatedSpecifiedDiscussionsCount(string userName, uint discussionsCount)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://user-microservice-api:8080/api/profile/User/GetUserByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return false;

            var user = await response.Content.ReadFromJsonAsync<UserResponse>();
            if (user.CreatedDiscussionsIds.Count >= discussionsCount) return true;
            return false;
        }

        public async Task<bool> HasUserCreatedSpecifiedCommentsCount(string userName, uint commentsCount)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://user-microservice-api:8080/api/profile/User/GetUserByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return false;

            var user = await response.Content.ReadFromJsonAsync<UserResponse>();
            if (user.CommentsIds.Count >= commentsCount) return true;
            return false;
        }
    }
}
