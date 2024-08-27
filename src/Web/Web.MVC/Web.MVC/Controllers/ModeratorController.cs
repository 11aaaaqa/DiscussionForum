using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Models.ApiResponses;

namespace Web.MVC.Controllers
{
    public class ModeratorController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ModeratorController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> SuggestedTopics()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync("http://topic-microservice-api:8080/api/SuggestTopic/GetAllSuggestedTopics");

            var content = await response.Content.ReadFromJsonAsync<List<TopicResponse>>();

            return View(content);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptSuggestedTopic(Guid id)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            await httpClient.DeleteAsync($"http://topic-microservice-api:8080/api/SuggestTopic/AcceptSuggestedTopic/{id}");
            return RedirectToAction("SuggestedTopics");
        }

        [HttpPost]
        public async Task<IActionResult> RejectSuggestedTopic(Guid id)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            
            await httpClient.DeleteAsync($"http://topic-microservice-api:8080/api/SuggestTopic/RejectSuggestedTopic/{id}");
            return RedirectToAction("SuggestedTopics");
        }
    }
}
