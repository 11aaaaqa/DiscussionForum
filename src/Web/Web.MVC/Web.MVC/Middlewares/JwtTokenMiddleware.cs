using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using GeneralClassesLib.ApiResponses;
using Web.MVC.Models.ApiRequests;
using Web.MVC.Models.ApiResponses;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Web.MVC.Middlewares
{
    public class JwtTokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;

        public JwtTokenMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.next = next;
            this.httpClientFactory = httpClientFactory;
            url = (string.IsNullOrEmpty(config["Url:Port"]))
                ? $"{config["Url:Protocol"]}://{config["Url:HostName"]}" : $"{config["Url:Protocol"]}://{config["Url:HostName"]}:{config["Url:Port"]}";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.Cookies.TryGetValue("accessToken", out var accessToken);
            if (accessToken != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(accessToken);

                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    var currentUserName = jwtToken.Claims.Single(x => x.Type == ClaimTypes.Name).Value;

                    using var httpClient = httpClientFactory.CreateClient();
                    var response = await httpClient.GetAsync(
                        $"{url}/api/User/GetByUserName?userName={currentUserName}");
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                        await next(context);
                    

                    var currentUser = await response.Content.ReadFromJsonAsync<GetUserResponse>();
                    if (currentUser.RefreshTokenExpiryTime < DateTime.UtcNow)
                    {
                        await next(context);
                    }

                    var tokenApiModel = new TokenApiModel
                        { AccessToken = accessToken, RefreshToken = currentUser.RefreshToken };
                    using StringContent jsonContent = new(JsonSerializer.Serialize(tokenApiModel), Encoding.UTF8, "application/json");
                    var refreshResponse = await httpClient.PostAsync(
                        $"{url}/api/Token/refresh", jsonContent);
                    var refreshContent = await refreshResponse.Content.ReadFromJsonAsync<AuthenticatedResponse>();

                    context.Response.Cookies.Append("accessToken", refreshContent.Token, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddMonths(2),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    context.Request.Headers.Add("Authorization", "Bearer " + refreshContent.Token);
                }
                else
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + accessToken);
                }
            }

            await next(context);
        }
    }
}
