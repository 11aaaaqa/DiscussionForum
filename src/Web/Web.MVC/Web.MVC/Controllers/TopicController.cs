using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;
using Web.MVC.DTOs.Topic;
using Web.MVC.Models;
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

        [Route("topics/{topicName}")]
        [HttpGet]
        public async Task<IActionResult> Topic(string topicName)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var result =
                await httpClient.GetAsync($"http://topic-microservice-api:8080/api/Topic/GetByName?name={topicName}");
            if (result.StatusCode == HttpStatusCode.OK)
            {
                var topic = await result.Content.ReadFromJsonAsync<TopicResponse>();
                var discussionResponse = await httpClient.GetAsync(
                    $"http://discussion-microservice-api:8080/api/Discussion/GetDiscussionsByTopicName?topicName={topic.Name}");
                var discussions = await discussionResponse.Content.ReadFromJsonAsync<List<DiscussionResponse>>();
                var discussionViewModel = new DiscussionViewModel { Discussions = discussions, TopicName = topicName };
                return View(discussionViewModel);
            }
            return View("ActionError");
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

                var getLink =
                    $"http://user-microservice-api:8080/api/profile/User/IsUserBannedByUserName/{User.Identity.Name}?banTypes[]={BanTypeConstants.GeneralBanType}&banTypes[]={BanTypeConstants.TopicBanType}";

                var isUserBannedResponse = await httpClient.GetAsync(getLink);
                if (!isUserBannedResponse.IsSuccessStatusCode) return View("ActionError");
                
                var isUserBanned = await isUserBannedResponse.Content.ReadFromJsonAsync<bool>();
                if (isUserBanned) return View("TopicBanned");
                
                using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(
                    "http://topic-microservice-api:8080/api/SuggestTopic/SuggestTopic",
                    jsonContent);
                if (response.StatusCode == HttpStatusCode.OK)
                    return View("Thanks");

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                ModelState.AddModelError(string.Empty, error.Reason);
            }
            return View(model);
        }
    }
}
