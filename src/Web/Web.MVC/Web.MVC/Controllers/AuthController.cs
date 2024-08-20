using System.Net;
using System.Text;
using System.Text.Json;
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
                    ViewBag.ConfirmEmailString = content;
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
    }
}
