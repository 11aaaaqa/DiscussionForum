using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Topic;
using Web.MVC.Models.ApiResponses;
using StringContent = System.Net.Http.StringContent;

namespace Web.MVC.Controllers
{
    public class TopicController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public TopicController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Topics()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var result = await httpClient.GetAsync("http://topic-microservice-api:8080/api/Topic/GetAll");

            var content = await result.Content.ReadFromJsonAsync<List<TopicResponse>>();

            return View(content);
        }

        [Authorize]
        [HttpGet]
        public IActionResult SuggestTopic()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SuggestTopic(TopicDto model)
        {
            if (ModelState.IsValid)
            {
                using var httpClient = httpClientFactory.CreateClient();

                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(
                    "http://topic-microservice-api:8080/api/SuggestTopic/SuggestTopic",
                    jsonContent);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return View("Thanks");
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    ModelState.AddModelError(string.Empty, error.Reason);
                }
            }
            return View(model);
        }
    }
}
