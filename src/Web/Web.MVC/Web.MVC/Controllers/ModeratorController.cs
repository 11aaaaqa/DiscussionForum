using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;
using Web.MVC.DTOs.Moderator;
using Web.MVC.Models;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.ApNetUserResponses;
using Web.MVC.Models.ApiResponses.CommentsResponses;
using Web.MVC.Models.ApiResponses.CustUserResponses;
using Web.MVC.Models.ApiResponses.Discussion;
using Web.MVC.Models.ApiResponses.ReportResponses;
using Web.MVC.Services;
using DiscussionResponse = Web.MVC.Models.ApiResponses.DiscussionResponse;

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
        public async Task<IActionResult> AcceptSuggestedTopic(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.DeleteAsync($"http://topic-microservice-api:8080/api/SuggestTopic/AcceptSuggestedTopic/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("SuggestedTopics");
        }

        [HttpPost]
        public async Task<IActionResult> RejectSuggestedTopic(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            
            var response = await httpClient.DeleteAsync($"http://topic-microservice-api:8080/api/SuggestTopic/RejectSuggestedTopic/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("SuggestedTopics");
        }

        [HttpGet]
        public async Task<IActionResult> SuggestedDiscussions()
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(
                    "http://discussion-microservice-api:8080/api/SuggestDiscussion/GetAllSuggestedDiscussions");

            if (!response.IsSuccessStatusCode) return View("ActionError");

            var discussions = await response.Content.ReadFromJsonAsync<List<DiscussionResponse>>();
            return View(discussions);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptSuggestedDiscussion(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.DeleteAsync($"http://discussion-microservice-api:8080/api/SuggestDiscussion/AcceptSuggestedDiscussion/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("SuggestedDiscussions");
        }

        [HttpPost]
        public async Task<IActionResult> RejectSuggestedDiscussion(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.DeleteAsync($"http://discussion-microservice-api:8080/api/SuggestDiscussion/RejectSuggestedDiscussion/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if(!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("SuggestedDiscussions");
        }

        [HttpGet]
        [Route("SuggestedDiscussions/{id}")]
        public async Task<IActionResult> SuggestedDiscussion(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/SuggestDiscussion/GetSuggestedDiscussionById?id={id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.ReturnUrl = returnUrl;
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
        public async Task<IActionResult> AcceptSuggestedComment(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"http://comment-microservice-api:8080/api/SuggestComment/AcceptSuggestedComment/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RejectSuggestedComment(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"http://comment-microservice-api:8080/api/SuggestComment/RejectSuggestedComment/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> BanUser(string? userName, Guid? userId)
        {
            if (userName is null && userId is null) return View("ActionError");

            using HttpClient httpClient = httpClientFactory.CreateClient();
            if (userName is null)
            {
                var response = await httpClient.GetAsync($"http://user-microservice-api:8080/api/profile/User/UserBanInfoByUserId/{userId}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                var banInfo = await response.Content.ReadFromJsonAsync<UserBanInfoResponse>();
                ViewBag.IsUserBanned = banInfo.IsBanned;
                if (banInfo.IsBanned)
                {
                    ViewBag.BanReason = banInfo.BanReason;
                    ViewBag.BanType = banInfo.BanType;
                    ViewBag.BannedUntil = banInfo.BannedUntil;
                }
            }
            else
            {
                var response = await httpClient.GetAsync($"http://user-microservice-api:8080/api/profile/User/UserBanInfoByUserName/{userName}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                var banInfo = await response.Content.ReadFromJsonAsync<UserBanInfoResponse>();
                ViewBag.IsUserBanned = banInfo.IsBanned;
                if (banInfo.IsBanned)
                {
                    ViewBag.BanReason = banInfo.BanReason;
                    ViewBag.BanType = banInfo.BanType;
                    ViewBag.BannedUntil = banInfo.BannedUntil;
                }
            }
            
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
                    var response = await httpClient.PatchAsync(
                        $"http://user-microservice-api:8080/api/profile/User/BanUserByUserName/{userName}", jsonContent);
                    if (!response.IsSuccessStatusCode) return View("ActionError");
                }
                else
                {
                    using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PatchAsync(
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
        public async Task<IActionResult> BanUserAndDeleteSuggestion(string? userName, Guid? userId)
        {
            if (userName is null && userId is null) return View("ActionError");

            using HttpClient httpClient = httpClientFactory.CreateClient();
            if (userName is null)
            {
                var response = await httpClient.GetAsync($"http://user-microservice-api:8080/api/profile/User/UserBanInfoByUserId/{userId}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                var banInfo = await response.Content.ReadFromJsonAsync<UserBanInfoResponse>();
                ViewBag.IsUserBanned = banInfo.IsBanned;
                if (banInfo.IsBanned)
                {
                    ViewBag.BanReason = banInfo.BanReason;
                    ViewBag.BanType = banInfo.BanType;
                    ViewBag.BannedUntil = banInfo.BannedUntil;
                }
            }
            else
            {
                var response = await httpClient.GetAsync($"http://user-microservice-api:8080/api/profile/User/UserBanInfoByUserName/{userName}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                var banInfo = await response.Content.ReadFromJsonAsync<UserBanInfoResponse>();
                ViewBag.IsUserBanned = banInfo.IsBanned;
                if (banInfo.IsBanned)
                {
                    ViewBag.BanReason = banInfo.BanReason;
                    ViewBag.BanType = banInfo.BanType;
                    ViewBag.BannedUntil = banInfo.BannedUntil;
                }
            }

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
                    var response = await httpClient.PatchAsync(
                        $"http://user-microservice-api:8080/api/profile/User/BanUserByUserName/{userName}", jsonContent);
                    if (!response.IsSuccessStatusCode) return View("ActionError");
                }
                else
                {
                    using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PatchAsync(
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

        [Route("SuggestedTopics/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedTopicsByUserName(string userName)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://topic-microservice-api:8080/api/SuggestTopic/GetSuggestedTopicsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            ViewBag.SuggestedByUserName = userName;
            var suggestedTopics = await response.Content.ReadFromJsonAsync<List<TopicResponse>>();
            return View(suggestedTopics);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSuggestedTopicsByUserName(string userName, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://topic-microservice-api:8080/api/SuggestTopic/DeleteAllSuggestedTopicsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Route("Suggestions/Discussions/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedDiscussionsByUserName(string userName)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/SuggestDiscussion/GetSuggestedDiscussionsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            ViewBag.SuggestedByUserName = userName;
            var suggestedDiscussions = await response.Content.ReadFromJsonAsync<List<SuggestedDiscussionResponse>>();
            return View(suggestedDiscussions);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllSuggestedDiscussionsByUserName(string userName, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://discussion-microservice-api:8080/api/SuggestDiscussion/DeleteAllSuggestedDiscussionsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Route("Suggestions/Comments/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedCommentsByUserName(string userName)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://comment-microservice-api:8080/api/SuggestComment/GetSuggestedCommentsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            ViewBag.SuggestedByUserName = userName;
            var suggestedComments = await response.Content.ReadFromJsonAsync<List<SuggestedCommentResponse>>();
            return View(suggestedComments);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllSuggestedCommentsByUserName(string userName, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://comment-microservice-api:8080/api/SuggestComment/DeleteAllSuggestedCommentsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Route("ReportsByUser/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetReportsByUserName(string userName, int pageNumber, int pageSize)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://report-microservice-api:8080/api/Report/GetReportsByUserName/{userName}?pageNumber={pageNumber}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            ViewBag.ReportedByUserName = userName;
            var reports = await response.Content.ReadFromJsonAsync<List<ReportApiResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"http://report-microservice-api:8080/api/Report/DoesNextPageByUserNameExist?userName={userName}&pageNumber={pageNumber + 1}&pageSize={pageSize}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetReportsViewModel
            {
                PageSize = pageSize, CurrentPageNumber = pageNumber, DoesNextPageExist = doesExist, NextPageNumber = pageNumber + 1, 
                PreviousPageNumber = pageNumber - 1, Reports = reports
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllReportsByUserName(string userName, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://report-microservice-api:8080/api/Report/DeleteReportsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ChangeUserRoles(string aspNetUserId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"http://register-microservice-api:8080/api/User/GetUserRolesByUserId/{aspNetUserId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");
            var roles = await response.Content.ReadFromJsonAsync<List<string>>();

            var model = new List<CheckBoxUserRoleDto>
            {
                 new(){IsChecked = false, RoleName = UserRoleConstants.AdminRole},
                 new(){IsChecked = false, RoleName = UserRoleConstants.ModeratorRole}
            };
            foreach (var role in roles)
            {
                switch (role)
                {
                    case UserRoleConstants.AdminRole:
                        model[0].IsChecked = true;
                        break;
                    case UserRoleConstants.ModeratorRole:
                        model[1].IsChecked = true;
                        break;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRoles(string aspNetUserId, string returnUrl, List<CheckBoxUserRoleDto> userRoles)
        {
            var selectedUserRolesCheckBox = userRoles.Where(x => x.IsChecked).ToList();
            var selectedUserRoles = new List<string>();
            foreach (var selectedUserRoleCheckBox in selectedUserRolesCheckBox)
            {
                selectedUserRoles.Add(selectedUserRoleCheckBox.RoleName);
            }

            using StringContent jsonContent = new(JsonSerializer.Serialize(new { RoleNames = selectedUserRoles, UserId = aspNetUserId}),
                Encoding.UTF8, "application/json");
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync("http://register-microservice-api:8080/api/User/AddUserToRoles", jsonContent);
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Users(int pageNumber, int pageSize, string? searchingQuery, string searchingType)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            bool doesExist;
            List<AspNetUserResponse>? users;
            if (searchingQuery is null)
            {
                var response = await httpClient.GetAsync(
                $"http://register-microservice-api:8080/api/User/GetAllUsers?pageNumber={pageNumber}&pageSize={pageSize}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                users = await response.Content.ReadFromJsonAsync<List<AspNetUserResponse>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"http://register-microservice-api:8080/api/User/DoesNextUsersPageExist?pageNumber={pageNumber + 1}&pageSize={pageSize}");
                if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");
                doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }
            else
            {
                var response = await httpClient.GetAsync(
                    $"http://register-microservice-api:8080/api/User/GetAllUsersSearching?pageNumber={pageNumber}&pageSize={pageSize}&searchingString={searchingQuery}&searchingType={searchingType}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                users = await response.Content.ReadFromJsonAsync<List<AspNetUserResponse>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"http://register-microservice-api:8080/api/User/DoesNextUsersPageSearchingExist?pageNumber={pageNumber + 1}&pageSize={pageSize}&searchingString={searchingQuery}&searchingType={searchingType}");
                if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

                doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
                ViewBag.SearchingQuery = searchingQuery;
                ViewBag.SearchingType = searchingType;
            }

            ViewBag.DoesNextPageExist = doesExist;
            ViewBag.NextPageNumber = pageNumber + 1;
            ViewBag.PreviousPageNumber = pageNumber - 1;
            ViewBag.CurrentPageSize = pageSize;

            return View(users);
        }
    }
}
