using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Web.MVC.Constants;
using Web.MVC.DTOs.Moderator;
using Web.MVC.Models;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.ApNetUserResponses;
using Web.MVC.Models.ApiResponses.CommentsResponses;
using Web.MVC.Models.ApiResponses.CustUserResponses;
using Web.MVC.Models.ApiResponses.Discussion;
using Web.MVC.Models.ApiResponses.ReportResponses;
using Web.MVC.Models.ViewModels.ModeratorViewModels;
using Web.MVC.Models.ViewModels.UserViewModels;
using Web.MVC.Services;

namespace Web.MVC.Controllers
{
    [Authorize(Roles = UserRoleConstants.AdminRole + ", " + UserRoleConstants.ModeratorRole)]
    public class ModeratorController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IReportService reportService;
        private readonly ISuggestionService suggestionService;
        private readonly string url;
        private static readonly Counter AcceptedDiscussionsCounter = Metrics.CreateCounter(
            "accepted_discussions", "Number of accepted discussions.");
        private static readonly Counter RejectedDiscussionsCounter = Metrics.CreateCounter(
            "rejected_discussions", "Number of rejected discussions.");
        private static readonly Counter AcceptedCommentsCounter = Metrics.CreateCounter(
            "accepted_comments", "Number of accepted comments.");
        private static readonly Counter RejectedCommentsCounter = Metrics.CreateCounter(
            "rejected_comments", "Number of rejected comments.");

        public ModeratorController(IHttpClientFactory httpClientFactory, IReportService reportService, ISuggestionService suggestionService,
            IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            this.reportService = reportService;
            this.suggestionService = suggestionService;
            url = (string.IsNullOrEmpty(config["Url:Port"]))
                ? $"{config["Url:Protocol"]}://{config["Url:HostName"]}" : $"{config["Url:Protocol"]}://{config["Url:HostName"]}:{config["Url:Port"]}";
        }

        [Route("admin-panel")]
        [HttpGet]
        public IActionResult GetAdminPanel()
        {
            return View();
        }

        [Route("moderator/suggested/topics")]
        [HttpGet]
        public async Task<IActionResult> SuggestedTopics(int pageSize, int pageNumber)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(
                $"{url}/api/SuggestTopic/GetAllSuggestedTopics?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");
            var suggestedTopics = await response.Content.ReadFromJsonAsync<List<TopicResponse>>();

            var doesNextSuggestedTopicsPageExist = await httpClient.GetAsync(
                $"{url}/api/SuggestTopic/DoesNextSuggestedTopicsPageExist?pageNumber={pageNumber + 1}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextSuggestedTopicsPageExist.Content.ReadFromJsonAsync<bool>();

            return View(new GetSuggestedTopicViewModel
            {
                PageSize = pageSize, NextPageNumber = pageNumber + 1, DoesNextPageExist = doesExist, CurrentPageNumber = pageNumber,
                PreviousPageNumber = pageNumber - 1, SuggestedTopics = suggestedTopics
            });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptSuggestedTopic(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.DeleteAsync($"{url}/api/SuggestTopic/AcceptSuggestedTopic/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("SuggestedTopics");
        }

        [HttpPost]
        public async Task<IActionResult> RejectSuggestedTopic(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            
            var response = await httpClient.DeleteAsync($"{url}/api/SuggestTopic/RejectSuggestedTopic/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("SuggestedTopics");
        }

        [Route("moderator/suggested/discussions")]
        [HttpGet]
        public async Task<IActionResult> SuggestedDiscussions(int pageSize, int pageNumber)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(
                $"{url}/api/SuggestDiscussion/GetAllSuggestedDiscussions?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var discussions = await response.Content.ReadFromJsonAsync<List<DiscussionResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/SuggestDiscussion/DoesNextAllSuggestedDiscussionsPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetSuggestedDiscussionsViewModel
            {
                PageSize = pageSize, CurrentPageNumber = pageNumber, DoesNextPageExist = doesExist,NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1, SuggestedDiscussions = discussions
            });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptSuggestedDiscussion(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.DeleteAsync($"{url}/api/SuggestDiscussion/AcceptSuggestedDiscussion/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            AcceptedDiscussionsCounter.Inc();

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("SuggestedDiscussions");
        }

        [HttpPost]
        public async Task<IActionResult> RejectSuggestedDiscussion(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.DeleteAsync($"{url}/api/SuggestDiscussion/RejectSuggestedDiscussion/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            RejectedDiscussionsCounter.Inc();

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
                $"{url}/api/SuggestDiscussion/GetSuggestedDiscussionById?id={id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.ReturnUrl = returnUrl;
                var suggestedDiscussion = await response.Content.ReadFromJsonAsync<DiscussionResponse>();
                return View(suggestedDiscussion);
            }

            return View("ActionError");
        }

        [Route("moderator/suggested/comments")]
        [HttpGet]
        public async Task<IActionResult> SuggestedComments(int pageSize, int pageNumber)
        {
            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"{url}/api/SuggestComment/GetAllSuggestedComments?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");
            
            var suggestedComments = await response.Content.ReadFromJsonAsync<List<SuggestedCommentResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/SuggestComment/DoesNextSuggestedCommentsPageExist?pageNumber={pageNumber + 1}&pageSize={pageSize}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new SuggestedCommentsViewModel
            {
                SuggestedComments = suggestedComments, PageSize = pageSize, CurrentPageNumber = pageNumber, DoesNextPageExist = doesExist,
                NextPageNumber = pageNumber + 1, PreviousPageNumber = pageNumber - 1
            });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptSuggestedComment(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"{url}/api/SuggestComment/AcceptSuggestedComment/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            AcceptedCommentsCounter.Inc();

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RejectSuggestedComment(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"{url}/api/SuggestComment/RejectSuggestedComment/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            RejectedCommentsCounter.Inc();

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Route("moderator/ban-user")]
        [HttpGet]
        public async Task<IActionResult> BanUser(string? userName, Guid? userId)
        {
            if (userName is null && userId is null) return View("ActionError");

            using HttpClient httpClient = httpClientFactory.CreateClient();
            if (userName is null)
            {
                var response = await httpClient.GetAsync($"{url}/api/profile/User/UserBanInfoByUserId/{userId}");
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
                var response = await httpClient.GetAsync($"{url}/api/profile/User/UserBanInfoByUserName/{userName}");
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

        [Route("moderator/ban-user")]
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
                model.BannedBy = User.Identity.Name;
                using HttpClient httpClient = httpClientFactory.CreateClient();

                if (userId is null)
                {
                    using StringContent jsonContent =
                        new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PatchAsync(
                        $"{url}/api/profile/User/BanUserByUserName/{userName}", jsonContent);
                    if (!response.IsSuccessStatusCode) return View("ActionError");
                }
                else
                {
                    using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PatchAsync(
                        $"{url}/api/profile/User/BanUserByUserId/{userId}", jsonContent);
                    if (!response.IsSuccessStatusCode) return View("ActionError");
                }

                if (!string.IsNullOrEmpty(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [Route("moderator/ban-user-and-delete-suggestion")]
        [HttpGet]
        public async Task<IActionResult> BanUserAndDeleteSuggestion(string? userName, Guid? userId)
        {
            if (userName is null && userId is null) return View("ActionError");

            using HttpClient httpClient = httpClientFactory.CreateClient();
            if (userName is null)
            {
                var response = await httpClient.GetAsync($"{url}/api/profile/User/UserBanInfoByUserId/{userId}");
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
                var response = await httpClient.GetAsync($"{url}/api/profile/User/UserBanInfoByUserName/{userName}");
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

        [Route("moderator/ban-user-and-delete-suggestion")]
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
                model.BannedBy = User.Identity.Name;
                using HttpClient httpClient = httpClientFactory.CreateClient();

                if (userId is null)
                {
                    using StringContent jsonContent =
                        new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PatchAsync(
                        $"{url}/api/profile/User/BanUserByUserName/{userName}", jsonContent);
                    if (!response.IsSuccessStatusCode) return View("ActionError");
                }
                else
                {
                    using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PatchAsync(
                        $"{url}/api/profile/User/BanUserByUserId/{userId}", jsonContent);
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
        public async Task<IActionResult> GetSuggestedTopicsByUserName(string userName, int pageSize, int pageNumber)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"{url}/api/SuggestTopic/GetSuggestedTopicsByUserName/{userName}?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/SuggestTopic/DoesNextSuggestedTopicsByUserNamePageExist/{userName}?pageNumber={pageNumber + 1}&pageSize={pageSize}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            var suggestedTopics = await response.Content.ReadFromJsonAsync<List<TopicResponse>>();
            return View(new SuggestedTopicsByUserNameViewModel
            {
                PageSize = pageSize,DoesNextPageExist = doesNextPageExist, CurrentPageNumber = pageNumber,NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1, SuggestedTopics = suggestedTopics, SuggestedByUserName = userName
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSuggestedTopicsByUserName(string userName, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"{url}/api/SuggestTopic/DeleteAllSuggestedTopicsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Route("Suggestions/Discussions/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedDiscussionsByUserName(string userName, int pageSize, int pageNumber)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"{url}/api/SuggestDiscussion/GetSuggestedDiscussionsByUserName/{userName}?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/SuggestDiscussion/DoesNextSuggestedDiscussionsByUserNamePageExist/{userName}?pageNumber={pageNumber + 1}&pageSize={pageSize}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            var suggestedDiscussions = await response.Content.ReadFromJsonAsync<List<SuggestedDiscussionResponse>>();
            return View(new GetSuggestedDiscussionsByUserNameViewModel
            {
                PageSize = pageSize, CurrentPageNumber = pageNumber, DoesNextPageExist = doesNextPageExist, NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1, SuggestedByUserName = userName, SuggestedDiscussions = suggestedDiscussions
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllSuggestedDiscussionsByUserName(string userName, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"{url}/api/SuggestDiscussion/DeleteAllSuggestedDiscussionsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Route("Suggestions/Comments/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedCommentsByUserName(string userName, int pageNumber, int pageSize)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"{url}/api/SuggestComment/GetSuggestedCommentsByUserName/{userName}?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/SuggestComment/DoesNextSuggestedCommentsByUserNamePageExist/{userName}?pageNumber={pageNumber + 1}&pageSize={pageSize}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            var suggestedComments = await response.Content.ReadFromJsonAsync<List<SuggestedCommentResponse>>();
            return View(new GetSuggestedCommentsByUserNameViewModel
            {
                PageSize = pageSize, DoesNextPageExist = doesNextPageExist, CurrentPageNumber = pageNumber, NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1, SuggestedByUserName = userName, SuggestedComments = suggestedComments
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllSuggestedCommentsByUserName(string userName, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"{url}/api/SuggestComment/DeleteAllSuggestedCommentsByUserName/{userName}");
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
                $"{url}/api/Report/GetReportsByUserName/{userName}?pageNumber={pageNumber}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            ViewBag.ReportedByUserName = userName;
            var reports = await response.Content.ReadFromJsonAsync<List<ReportApiResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/Report/DoesNextPageByUserNameExist?userName={userName}&pageNumber={pageNumber + 1}&pageSize={pageSize}");
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
                $"{url}/api/Report/DeleteReportsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Route("moderator/change-user-roles")]
        [HttpGet]
        public async Task<IActionResult> ChangeUserRoles(string aspNetUserId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{url}/api/User/GetUserRolesByUserId/{aspNetUserId}");
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

        [Route("moderator/change-user-roles")]
        [HttpPost]
        public async Task<IActionResult> ChangeUserRoles(string aspNetUserId, string returnUrl, List<CheckBoxUserRoleDto> userRoles)
        {
            var selectedUserRolesCheckBox = userRoles.Where(x => x.IsChecked).ToList();
            var selectedUserRoles = new List<string>();
            foreach (var selectedUserRoleCheckBox in selectedUserRolesCheckBox)
            {
                selectedUserRoles.Add(selectedUserRoleCheckBox.RoleName);
            }

            using StringContent jsonContent = new(JsonSerializer.Serialize(new { RoleNames = selectedUserRoles, UserId = aspNetUserId }),
                Encoding.UTF8, "application/json");
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync($"{url}/api/User/AddUserToRoles", jsonContent);
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var resetUserRefreshTokenResponse = await httpClient.GetAsync($"{url}/api/User/LogUserOut?userId={aspNetUserId}");
            if (!resetUserRefreshTokenResponse.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Route("moderator/users")]
        [HttpGet]
        public async Task<IActionResult> Users(int pageNumber, int pageSize, string? searchingQuery)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            bool doesExist;
            List<AspNetUserResponse>? users;
            if (searchingQuery is null)
            {
                var response = await httpClient.GetAsync(
                $"{url}/api/User/GetAllUsers?pageNumber={pageNumber}&pageSize={pageSize}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                users = await response.Content.ReadFromJsonAsync<List<AspNetUserResponse>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"{url}/api/User/DoesNextUsersPageExist?pageNumber={pageNumber + 1}&pageSize={pageSize}");
                if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");
                doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
            }
            else
            {
                var response = await httpClient.GetAsync(
                    $"{url}/api/User/GetAllUsersSearching?pageNumber={pageNumber}&pageSize={pageSize}&searchingString={searchingQuery}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                users = await response.Content.ReadFromJsonAsync<List<AspNetUserResponse>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"{url}/api/User/DoesNextUsersPageSearchingExist?pageNumber={pageNumber + 1}&pageSize={pageSize}&searchingString={searchingQuery}");
                if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

                doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
                ViewBag.SearchingQuery = searchingQuery;
            }

            ViewBag.DoesNextPageExist = doesExist;
            ViewBag.NextPageNumber = pageNumber + 1;
            ViewBag.PreviousPageNumber = pageNumber - 1;
            ViewBag.CurrentPageSize = pageSize;

            return View(users);
        }
    }
}