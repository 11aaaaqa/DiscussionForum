using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;
using Web.MVC.DTOs.Topic;
using Web.MVC.Models;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.Discussion;
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

        [Route("topics")]
        [HttpGet]
        public async Task<IActionResult> Topics(int pageSize, int pageNumber, string? searchingQuery)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            List<TopicResponse> topics;
            bool doesExist;
            if (searchingQuery is null)
            {
                var response = await httpClient.GetAsync(
                    $"http://topic-microservice-api:8080/api/Topic/GetAll?pageSize={pageSize}&pageNumber={pageNumber}");
                if (!response.IsSuccessStatusCode) return View("ActionError");
                topics = await response.Content.ReadFromJsonAsync<List<TopicResponse>>();

                var doesExistResponse = await httpClient.GetAsync(
                    $"http://topic-microservice-api:8080/api/Topic/DoesNextTopicsPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}");
                if (!doesExistResponse.IsSuccessStatusCode) return View("ActionError");

                doesExist = await doesExistResponse.Content.ReadFromJsonAsync<bool>();
            }
            else
            {
                var response = await httpClient.GetAsync(
                    $"http://topic-microservice-api:8080/api/Topic/FindTopics?pageSize={pageSize}&pageNumber={pageNumber}&searchingString={searchingQuery}");
                if (!response.IsSuccessStatusCode) return View("ActionError");
                topics = await response.Content.ReadFromJsonAsync<List<TopicResponse>>();

                var doesExistResponse = await httpClient.GetAsync(
                    $"http://topic-microservice-api:8080/api/Topic/DoesNextTopicsPageExistSearching?pageSize={pageSize}&pageNumber={pageNumber + 1}&searchingString={searchingQuery}");
                if (!doesExistResponse.IsSuccessStatusCode) return View("ActionError");

                doesExist = await doesExistResponse.Content.ReadFromJsonAsync<bool>();
            }

            ViewBag.DoesNextPageExist = doesExist;
            ViewBag.NextPageNumber = pageNumber + 1;
            ViewBag.PreviousPageNumber = pageNumber - 1;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchingQuery = searchingQuery;

            return View(topics);
        }

        [Route("topics/{topicName}")]
        [HttpGet]
        public async Task<IActionResult> Topic(string topicName, int pageNumber, int pageSize, string? searchingQuery)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response =
                await httpClient.GetAsync($"http://topic-microservice-api:8080/api/Topic/GetByName?name={topicName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");
            var topic = await response.Content.ReadFromJsonAsync<TopicResponse>();
            DiscussionViewModel discussionViewModel;
            bool doesNextPageExist;

            if (searchingQuery is null)
            {
                var discussionResponse = await httpClient.GetAsync(
                    $"http://discussion-microservice-api:8080/api/Discussion/GetDiscussionsByTopicName?topicName={topic.Name}&pageSize={pageSize}&pageNumber={pageNumber}");
                if (!discussionResponse.IsSuccessStatusCode) return View("ActionError");
                var discussions = await discussionResponse.Content.ReadFromJsonAsync<List<DiscussionResponse>>();
                discussionViewModel = new DiscussionViewModel { Discussions = discussions, TopicName = topicName };

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"http://discussion-microservice-api:8080/api/Discussion/DoesNextDiscussionsPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}&topicName={topic.Name}");
                if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");
                doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }
            else
            {
                var discussionResponse = await httpClient.GetAsync(
                    $"http://discussion-microservice-api:8080/api/Discussion/FindDiscussionsByTopicNameBySearchingString?topicName={topic.Name}&pageSize={pageSize}&pageNumber={pageNumber}&searchingString={searchingQuery}");
                if (!discussionResponse.IsSuccessStatusCode) return View("ActionError");
                var discussions = await discussionResponse.Content.ReadFromJsonAsync<List<DiscussionResponse>>();
                discussionViewModel = new DiscussionViewModel { Discussions = discussions, TopicName = topicName };

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"http://discussion-microservice-api:8080/api/Discussion/DoesNextDiscussionsPageExistSearching?pageSize={pageSize}&pageNumber={pageNumber + 1}&searchingQuery={searchingQuery}&topicName={topic.Name}");
                if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");
                doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }

            ViewBag.DoesNextPageExist = doesNextPageExist;
            ViewBag.CurrentPageNumber = pageNumber;
            ViewBag.NextPageNumber = pageNumber + 1;
            ViewBag.PreviousPageNumber = pageNumber - 1;
            ViewBag.SearchingQuery = searchingQuery;
            ViewBag.PageSize = pageSize;

            return View(discussionViewModel);
        }

        [Route("topics/suggest")]
        [Authorize]
        [HttpGet]
        public IActionResult SuggestTopic()
        {
            return View();
        }

        [Route("topics/suggest")]
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

                model.SuggestedBy = User.Identity.Name;
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

        [Route("topics/new")]
        [HttpGet]
        public async Task<IActionResult> GetAllTopicsSortedByNovelty(int pageNumber, int pageSize)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://topic-microservice-api:8080/api/Topic/GetAllTopicsSortedByNovelty?pageNumber={pageNumber}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var topics = await response.Content.ReadFromJsonAsync<List<TopicModel>>();

            var doesExistResponse = await httpClient.GetAsync(
                $"http://topic-microservice-api:8080/api/Topic/DoesNextTopicsPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.PreviousPageNumber = pageNumber - 1;
            ViewBag.NextPageNumber = pageNumber + 1;
            ViewBag.PageSize = pageSize;
            ViewBag.DoesNextPageExist = doesExist;

            return View(topics);
        }

        [Route("topics/popular")]
        [HttpGet]
        public async Task<IActionResult> GetAllTopicsSortedByPopularity(int pageNumber, int pageSize)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://topic-microservice-api:8080/api/Topic/GetAllTopicsSortedByPopularity?pageNumber={pageNumber}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var topics = await response.Content.ReadFromJsonAsync<List<TopicModel>>();

            var doesExistResponse = await httpClient.GetAsync(
                $"http://topic-microservice-api:8080/api/Topic/DoesNextTopicsPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesExistResponse.Content.ReadFromJsonAsync<bool>();

            ViewBag.PreviousPageNumber = pageNumber - 1;
            ViewBag.NextPageNumber = pageNumber + 1;
            ViewBag.PageSize = pageSize;
            ViewBag.DoesNextPageExist = doesExist;

            return View(topics);
        }
    }
}
