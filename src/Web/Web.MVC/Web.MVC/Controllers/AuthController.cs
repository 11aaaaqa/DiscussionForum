using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RegisterMicroserviceLib.DTOs.Auth;

namespace Web.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<AuthController> logger;

        public AuthController(IHttpClientFactory httpClientFactory, ILogger<AuthController> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
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
                using HttpClient client = httpClientFactory.CreateClient("SslCertificateBypassIfIsNotProduction");

                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                logger.LogInformation("Http client was created and model was serialized");

                var response = await client.PostAsync(
                    $"https://localhost:8081/api/Auth/register?confirmEmailController=Auth&confirmEmailMethod={nameof(ConfirmEmail)}",
                    jsonContent);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    logger.LogInformation("Response has success status code");
                    var content = await response.Content.ReadAsStringAsync();
                    ViewBag.ConfirmEmailString = content;
                }
                else
                {
                    logger.LogError("Response hasn't success status code");
                    ModelState.AddModelError(string.Empty,response.ReasonPhrase ?? "Что-то пошло не так, попробуйте ещё раз");
                }
            }
            logger.LogError("Model state isn't valid, end method");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            logger.LogInformation("ConfirmEmail method start working");
            using HttpClient client = httpClientFactory.CreateClient("SslCertificateBypassIfIsNotProduction");
            using StringContent jsonContent = new StringContent(
                JsonSerializer.Serialize(new { userId = userId, token = token }), Encoding.UTF8, "application/json");

            logger.LogInformation("Http client was created and model was serialized");

            var response = await client.PostAsync("https://localhost:8081/api/Auth/confirmEmail", jsonContent);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                logger.LogInformation("Response has success status code, end method");
                return View();
            }
            else
            {
                logger.LogError("Response hasn't success status code, end method");
                return View("Error");
            }
        }
    }
}
