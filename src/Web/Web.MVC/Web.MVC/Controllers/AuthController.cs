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

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient client = httpClientFactory.CreateClient();

                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(
                    $"https://localhost:7012/api/Auth/register?confirmEmailController=Auth&confirmEmailMethod={nameof(ConfirmEmail)}",
                    jsonContent);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ViewBag.ConfirmEmailString = content;
                }
                else
                {
                    ModelState.AddModelError(string.Empty,response.ReasonPhrase ?? "Что-то пошло не так, попробуйте ещё раз");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            using HttpClient client = httpClientFactory.CreateClient();
            using StringContent jsonContent = new StringContent(
                JsonSerializer.Serialize(new { userId = userId, token = token }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("", jsonContent);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Error");
            }
        }
    }
}
