using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;
using Web.MVC.DTOs.Comment;
using Web.MVC.DTOs.Discussion;
using Web.MVC.Models;
using Web.MVC.Models.ApiResponses.CommentsResponses;
using Web.MVC.Models.ApiResponses.Discussion;
using Web.MVC.Services;

namespace Web.MVC.Controllers
{
    public class DiscussionController : Controller
    {
        private readonly ICheckUserService checkUserService;
        private readonly IHttpClientFactory httpClientFactory;

        public DiscussionController(IHttpClientFactory httpClientFactory, ICheckUserService checkUserService)
        {
            this.httpClientFactory = httpClientFactory;
            this.checkUserService = checkUserService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult SuggestDiscussion(string topicName)
        {
            return View(new DiscussionDto{TopicName = topicName});
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SuggestDiscussion(DiscussionDto model)
        {
            if (ModelState.IsValid)
            {
                using var httpClient = httpClientFactory.CreateClient();

                var getLink =
                    $"http://user-microservice-api:8080/api/profile/User/IsUserBannedByUserName/{User.Identity.Name}?banTypes[]={BanTypeConstants.GeneralBanType}&banTypes[]={BanTypeConstants.DiscussionBanType}";
                var isUserBannedResponse = await httpClient.GetAsync(getLink);
                if (!isUserBannedResponse.IsSuccessStatusCode) return View("ActionError");

                var isUserBanned = await isUserBannedResponse.Content.ReadFromJsonAsync<bool>();
                if (isUserBanned) return View("DiscussionBanned");

                var isUserAchieveNeededCreatedDiscussions =
                    await checkUserService.HasUserCreatedSpecifiedDiscussionsCount(User.Identity.Name, 3);
                if (!isUserAchieveNeededCreatedDiscussions)
                {
                    using StringContent jsonContent = new
                        (JsonSerializer.Serialize(new { model.Title, model.Content, CreatedBy = User.Identity.Name }), Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(
                        $"http://discussion-microservice-api:8080/api/SuggestDiscussion/SuggestToCreate?topicName={model.TopicName}",
                        jsonContent);
                    if (response.StatusCode == HttpStatusCode.OK)
                        return View("Thanks");
                }
                else
                {
                    using StringContent jsonContent = new(JsonSerializer.Serialize(new { model.Title, model.TopicName, model.Content,
                            CreatedBy = User.Identity.Name }), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(
                        "http://discussion-microservice-api:8080/api/Discussion/CreateDiscussion",
                        jsonContent);
                    if (!response.IsSuccessStatusCode) return View("ActionError");

                    var discussionId = await response.Content.ReadFromJsonAsync<Guid>();
                    return LocalRedirect($"/discussions/{discussionId}");
                }

                return View("ActionError");
            }

            return View(model);
        }

        [HttpGet]
        [Route("discussions/{id}")]
        public async Task<IActionResult> GetDiscussion(Guid id, int pageSize, int pageNumber)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response =
                await httpClient.GetAsync(
                    $"http://discussion-microservice-api:8080/api/Discussion/GetDiscussionById?id={id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var discussion = await response.Content.ReadFromJsonAsync<DiscussionResponse>();
            ViewBag.Content = discussion.Content; ViewBag.CreatedAt = discussion.CreatedAt; ViewBag.CreatedBy = discussion.CreatedBy;
            ViewBag.Rating = discussion.Rating; ViewBag.Title = discussion.Title; ViewBag.DiscussionId = id;

            var getCommentsResponse = await httpClient.GetAsync(
                $"http://comment-microservice-api:8080/api/Comment/GetCommentsByDiscussionId/{id}?pageNumber={pageNumber}&pageSize={pageSize}");
            if (!getCommentsResponse.IsSuccessStatusCode) return View("ActionError");

            var comments = await getCommentsResponse.Content.ReadFromJsonAsync<List<CommentResponse>>();
            ViewBag.Comments = comments;

            var doesNextCommentsPageExistResponse = await httpClient.GetAsync(
                $"http://comment-microservice-api:8080/api/Comment/DoesNextCommentsByDiscussionIdPageExist/{id}?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextCommentsPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesNextCommentsPageExist = await doesNextCommentsPageExistResponse.Content.ReadFromJsonAsync<bool>();
            ViewBag.DoesNextCommentsPageExist = doesNextCommentsPageExist;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPageNumber = pageNumber;
            ViewBag.NextPageNumber = pageNumber + 1;
            ViewBag.PreviousPageNumber = pageNumber - 1;

            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("discussions/{id}")]
        public async Task<IActionResult> SuggestComment(SuggestCommentDto model, Guid id, Guid? repliedOnCommentId, string? repliedOnCommentCreatedBy,
            string? repliedOnCommentContent)
        {
            if (ModelState.IsValid)
            {
                using var httpClient = httpClientFactory.CreateClient();

                var getLink =
                    $"http://user-microservice-api:8080/api/profile/User/IsUserBannedByUserName/{User.Identity.Name}?banTypes[]={BanTypeConstants.GeneralBanType}&banTypes[]={BanTypeConstants.CommentBanType}";
                var isUserBannedResponse = await httpClient.GetAsync(getLink);
                if (!isUserBannedResponse.IsSuccessStatusCode) return View("ActionError");
                
                var isUserBanned = await isUserBannedResponse.Content.ReadFromJsonAsync<bool>();
                if (isUserBanned) return View("CommentBanned", model: id);

                var discussionId = id;
                model.CreatedBy = User.Identity.Name;

                var isAllowedToDirectlyCreateComment = await checkUserService.HasUserCreatedSpecifiedCommentsCount(User.Identity.Name, 7);
                if (!isAllowedToDirectlyCreateComment)
                {
                    using StringContent jsonContent = new(JsonSerializer.Serialize(new
                    {
                        RepliedOnCommentId = repliedOnCommentId,
                        RepliedOnCommentCreatedBy = repliedOnCommentCreatedBy,
                        RepliedOnCommentContent = repliedOnCommentContent,
                        model.CreatedBy,
                        model.Content,
                        DiscussionId = discussionId
                    }), Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("http://comment-microservice-api:8080/api/SuggestComment/Suggest", jsonContent);
                    if (response.IsSuccessStatusCode)
                        return View("ThanksForComment", discussionId);
                    
                    return View("SomethingWentWrong", discussionId);
                }
                else
                {
                    using StringContent jsonContent = new(JsonSerializer.Serialize(new
                    {
                        RepliedOnCommentId = repliedOnCommentId,
                        RepliedOnCommentCreatedBy = repliedOnCommentCreatedBy,
                        RepliedOnCommentContent = repliedOnCommentContent,
                        discussionId, model.CreatedBy, model.Content }), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(
                        "http://comment-microservice-api:8080/api/Comment/CreateComment", jsonContent);
                    if (!response.IsSuccessStatusCode)
                        return View("SomethingWentWrong", discussionId);
                    return LocalRedirect($"/discussions/{id}?pageSize=20&pageNumber=1");
                }
            }
            return View("SomethingWentWrong", id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDiscussion(Guid discussionId, string? returnUrl, string? reportType)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/DeleteDiscussionById/{discussionId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            if (!string.IsNullOrEmpty(reportType))
                return RedirectToAction("Reports", "Report", new {reportType});

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> IncreaseDiscussionRatingByOne(Guid discussionId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new { discussionId, userNameIncreasedBy = User.Identity.Name }),
                Encoding.UTF8, "application/json");
            var response = await httpClient.PatchAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/IncreaseDiscussionRatingByOne", jsonContent);
            if (!response.IsSuccessStatusCode) return View("RatingIsAlreadyIncreased", model: returnUrl);

            if(!string.IsNullOrEmpty(returnUrl)) 
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DecreaseDiscussionRatingByOne(Guid discussionId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new { discussionId, userNameDecreasedBy = User.Identity.Name }),
                Encoding.UTF8, "application/json");
            var response = await httpClient.PatchAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/DecreaseDiscussionRatingByOne", jsonContent);
            if (!response.IsSuccessStatusCode) return View("RatingIsAlreadyDecreased", model: returnUrl);

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Route("discussions/new")]
        [HttpGet]
        public async Task<IActionResult> GetDiscussionsSortedByNovelty(int pageNumber, int pageSize)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/GetAllDiscussionsSortedByNovelty?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var discussions = await response.Content.ReadFromJsonAsync<List<DiscussionResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/DoesNextAllDiscussionsPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetDiscussionsViewModel
            {
                PageSize = pageSize, CurrentPageNumber = pageNumber, DoesNextPageExist = doesExist,
                NextPageNumber = pageNumber + 1, PreviousPageNumber = pageNumber - 1, Discussions = discussions
            });
        }

        [Route("discussions/top/today")]
        [HttpGet]
        public async Task<IActionResult> GetDiscussionsSortedByPopularityForToday(int pageNumber, int pageSize)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/GetAllDiscussionsSortedByPopularityForToday?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var discussions = await response.Content.ReadFromJsonAsync<List<DiscussionResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/DoesNextDiscussionsForTodayPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetDiscussionsViewModel
            {
                PageSize = pageSize,
                CurrentPageNumber = pageNumber,
                DoesNextPageExist = doesExist,
                NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1,
                Discussions = discussions
            });
        }

        [Route("discussions/top/week")]
        [HttpGet]
        public async Task<IActionResult> GetDiscussionsSortedByPopularityForWeek(int pageNumber, int pageSize)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/GetAllDiscussionsSortedByPopularityForWeek?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var discussions = await response.Content.ReadFromJsonAsync<List<DiscussionResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/DoesNextDiscussionsForWeekPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetDiscussionsViewModel
            {
                PageSize = pageSize,
                CurrentPageNumber = pageNumber,
                DoesNextPageExist = doesExist,
                NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1,
                Discussions = discussions
            });
        }

        [Route("discussions/top/month")]
        [HttpGet]
        public async Task<IActionResult> GetDiscussionsSortedByPopularityForMonth(int pageNumber, int pageSize)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/GetAllDiscussionsSortedByPopularityForMonth?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var discussions = await response.Content.ReadFromJsonAsync<List<DiscussionResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/DoesNextDiscussionsForMonthPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetDiscussionsViewModel
            {
                PageSize = pageSize,
                CurrentPageNumber = pageNumber,
                DoesNextPageExist = doesExist,
                NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1,
                Discussions = discussions
            });
        }

        [Route("discussions/top/all-time")]
        [HttpGet]
        public async Task<IActionResult> GetDiscussionsSortedByPopularityForAllTime(int pageNumber, int pageSize)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/GetAllDiscussionsSortedByPopularityForAllTime?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var discussions = await response.Content.ReadFromJsonAsync<List<DiscussionResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/DoesNextAllDiscussionsPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetDiscussionsViewModel
            {
                PageSize = pageSize,
                CurrentPageNumber = pageNumber,
                DoesNextPageExist = doesExist,
                NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1,
                Discussions = discussions
            });
        }
    }
}
