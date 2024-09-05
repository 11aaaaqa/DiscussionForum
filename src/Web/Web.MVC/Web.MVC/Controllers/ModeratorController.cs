using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.CommentsResponses;

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

        [HttpGet]
        public async Task<IActionResult> SuggestedDiscussions()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(
                    "http://discussion-microservice-api:8080/api/SuggestDiscussion/GetAllSuggestedDiscussions");
            var discussions = await response.Content.ReadFromJsonAsync<List<DiscussionResponse>>();
            return View(discussions);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptSuggestedDiscussion(Guid id)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            await httpClient.DeleteAsync($"http://discussion-microservice-api:8080/api/SuggestDiscussion/AcceptSuggestedDiscussion/{id}");
            return RedirectToAction("SuggestedDiscussions");
        }

        [HttpPost]
        public async Task<IActionResult> RejectSuggestedDiscussion(Guid id)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            await httpClient.DeleteAsync($"http://discussion-microservice-api:8080/api/SuggestDiscussion/RejectSuggestedDiscussion/{id}");
            return RedirectToAction("SuggestedDiscussions");
        }

        [HttpGet]
        [Route("SuggestedDiscussions/{id}")]
        public async Task<IActionResult> SuggestedDiscussion(Guid id)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/SuggestDiscussion/GetSuggestedDiscussionById?id={id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var suggestedDiscussion = await response.Content.ReadFromJsonAsync<DiscussionResponse>();
                return View(suggestedDiscussion);
            }

            return View("ActionError");
        }

        [HttpGet]
        public async Task<IActionResult> SuggestedComments()
        {
            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync("http://comment-microservice-api:8080/api/SuggestComment/GetAllSuggestedComments");
            if (response.IsSuccessStatusCode)
            {
                var suggestedComments = await response.Content.ReadFromJsonAsync<List<SuggestedCommentResponse>>();
                return View(suggestedComments);
            }
            return View("ActionError");
        }

        [HttpPost]
        public async Task<IActionResult> AcceptSuggestedComment(Guid id)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"http://comment-microservice-api:8080/api/SuggestComment/AcceptSuggestedComment/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("SuggestedComments");
            }
            return View("ActionError");
        }
    }
}
