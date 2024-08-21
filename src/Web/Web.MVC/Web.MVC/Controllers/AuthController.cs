using System.Net;
using System.Text;
using System.Text.Json;
using GeneralClassesLib.ApiResponses;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Auth;

namespace Web.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<AuthController> logger;
        private readonly IConfiguration configuration;

        public AuthController(IHttpClientFactory httpClientFactory, ILogger<AuthController> logger, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            logger.LogInformation("Register method start working");

            if (ModelState.IsValid)
            {
                using HttpClient client = httpClientFactory.CreateClient();

                model.Uri = new ConfirmEmailMethodUri
                {
                    Protocol = configuration["Uri:Protocol"],
                    DomainName = configuration["Uri:DomainName"],
                    Controller = "Auth",
                    Action = "ConfirmEmail"
                };

                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                logger.LogInformation("Http client was created and model was serialized");
                
                var response = await client.PostAsync(
                    "http://register-microservice-api:8080/api/Auth/register",
                    jsonContent);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    logger.LogInformation("Response has success status code");
                    var content = await response.Content.ReadAsStringAsync();
                    ViewBag.ConfirmEmailString = content; //todo: redirect to the page that says to confirm email
                }
                else if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    logger.LogError("User already exists, end method");
                    ModelState.AddModelError(string.Empty, "Такой пользователь уже существует");
                }
                else
                {
                    logger.LogError("Response hasn't success status code, end method");
                    ModelState.AddModelError(string.Empty,"Что-то пошло не так, попробуйте ещё раз");
                }
            }
            else
            {
                logger.LogError("Model state isn't valid, end method");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            logger.LogInformation("ConfirmEmail method start working");
            using HttpClient client = httpClientFactory.CreateClient();

            logger.LogInformation("Http client was created");

            var response = await client.GetAsync($"http://register-microservice-api:8080/api/Auth/confirmEmail?token={token}&userId={userId}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                logger.LogInformation("Response has success status code, end method");
                return View();
            }
            else
            {
                logger.LogError("Response hasn't success status code, end method");
                return View("ActionError");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                using HttpClient httpclient = httpClientFactory.CreateClient();
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                var response = await httpclient.PostAsync("http://register-microservice-api:8080/api/Auth/login", jsonContent);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadFromJsonAsync<AuthenticatedResponse>();

                    Response.Cookies.Append("accessToken",content!.Token, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddMinutes(2),
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Secure = true
                    });

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home"); //todo: return url
                }
                if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    logger.LogError("User with current password doesn't exist, end method");
                    ModelState.AddModelError(string.Empty, "Пользователя с таким паролем не существует");
                }
                else
                {
                    logger.LogCritical("Something went wrong, user wasn't authenticated, end method");
                    ModelState.AddModelError(string.Empty, "Что-то пошло не так, попробуйте ещё раз");
                }
            }
            else
            {
                logger.LogError("Model wasn't valid, end method");
            }
            return View(model);
        }
    }
}
