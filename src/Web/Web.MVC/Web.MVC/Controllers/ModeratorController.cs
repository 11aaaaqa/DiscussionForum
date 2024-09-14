using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Web.MVC.Constants;
using Web.MVC.DTOs.Moderator;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.CommentsResponses;
using Web.MVC.Services;

namespace Web.MVC.Controllers
{
    public class ModeratorController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IReportService reportService;
        private readonly ISuggestionService suggestionService;

        public ModeratorController(IHttpClientFactory httpClientFactory, IReportService reportService, ISuggestionService suggestionService)
        {
            this.httpClientFactory = httpClientFactory;
            this.reportService = reportService;
            this.suggestionService = suggestionService;
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

        [HttpPost]
        public async Task<IActionResult> RejectSuggestedComment(Guid id)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"http://comment-microservice-api:8080/api/SuggestComment/RejectSuggestedComment/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("SuggestedComments");
            }
            return View("ActionError");
        }

        [HttpGet]
        public IActionResult BanUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(string? userName, Guid? userId, BanUserDto model, string returnUrl, string? banType)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(banType))
                {
                    ModelState.AddModelError(string.Empty, "Выберите тип бана");
                    return View(model);
                }

                if (userName is null && userId is null)
                    return View("ActionError");

                model.BanType = banType;
                using HttpClient httpClient = httpClientFactory.CreateClient();

                if (userId is null)
                {
                    using StringContent jsonContent =
                        new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(
                        $"http://user-microservice-api:8080/api/profile/User/BanUserByUserName/{userName}", jsonContent);
                    if (!response.IsSuccessStatusCode) return View("ActionError");
                }
                else
                {
                    using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(
                        $"http://user-microservice-api:8080/api/profile/User/BanUserByUserId/{userId}", jsonContent);
                    if (!response.IsSuccessStatusCode) return View("ActionError");
                }

                if (!string.IsNullOrEmpty(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult BanUserAndDeleteSuggestion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BanUserAndDeleteSuggestion(string? userName, Guid? userId, BanUserDto model, string returnUrl, string? banType,
            Guid suggestionDeleteId, string suggestionDeleteType)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(banType))
                {
                    ModelState.AddModelError(string.Empty, "Выберите тип бана");
                    return View(model);
                }

                if (userName is null && userId is null)
                    return View("ActionError");

                model.BanType = banType;
                using HttpClient httpClient = httpClientFactory.CreateClient();

                if (userId is null)
                {
                    using StringContent jsonContent =
                        new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(
                        $"http://user-microservice-api:8080/api/profile/User/BanUserByUserName/{userName}", jsonContent);
                    if (!response.IsSuccessStatusCode) return View("ActionError");
                }
                else
                {
                    using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(
                        $"http://user-microservice-api:8080/api/profile/User/BanUserByUserId/{userId}", jsonContent);
                    if (!response.IsSuccessStatusCode) return View("ActionError");
                }

                switch (suggestionDeleteType)
                {
                    case BanTypeConstants.TopicBanType:
                    {
                        var isDeleted = await suggestionService.DeleteSuggestedTopic(suggestionDeleteId);
                        if (!isDeleted) return View("ActionError");
                        break;
                    }
                    case BanTypeConstants.CommentBanType:
                    {
                        var isDeleted = await suggestionService.DeleteSuggestedComment(suggestionDeleteId);
                        if (!isDeleted) return View("ActionError");
                        break;
                    }
                    case BanTypeConstants.DiscussionBanType:
                    {
                        var isDeleted = await suggestionService.DeleteSuggestedDiscussion(suggestionDeleteId);
                        if (!isDeleted) return View("ActionError");
                        break;
                    }
                    case BanTypeConstants.ReportBanType:
                    {
                        var isDeleted = await reportService.DeleteReport(suggestionDeleteId);
                        if (!isDeleted) return View("ActionError");
                        break;
                    }
                }
                
                
                if (!string.IsNullOrEmpty(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
