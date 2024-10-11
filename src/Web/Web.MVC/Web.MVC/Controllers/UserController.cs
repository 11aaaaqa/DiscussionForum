using System.Net;
using System.Text;
using System.Text.Json;
using GeneralClassesLib.ApiResponses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;
using Web.MVC.DTOs.User;
using Web.MVC.Models;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.CommentsResponses;
using Web.MVC.Models.ApiResponses.Discussion;
using Web.MVC.Models.ViewModels.UserViewModels;
using DiscussionResponse = Web.MVC.Models.ApiResponses.Discussion.DiscussionResponse;
using UserResponse = Web.MVC.Models.ApiResponses.CustUserResponses.UserResponse;

namespace Web.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Route("users/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetUser(string userName)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"http://user-microservice-api:8080/api/profile/User/GetUserByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var user = await response.Content.ReadFromJsonAsync<UserResponse>();
            
            #region GettingDiscussions

            StringBuilder sb = new StringBuilder("http://discussion-microservice-api:8080/api/Discussion/GetDiscussionsByIds?");
            foreach (var discussionId in user.CreatedDiscussionsIds)
            {
                sb.Append($"ids[]={discussionId}&");
            }

            sb.Remove(sb.Length - 1, 1);
            var createdDiscussionsResponse = await httpClient.GetAsync(sb.ToString());
            if (!createdDiscussionsResponse.IsSuccessStatusCode) return View("ActionError");

            var discussions = await createdDiscussionsResponse.Content.ReadFromJsonAsync<List<DiscussionResponse>>();
            ViewBag.Discussions = discussions;

            #endregion

            #region GettingComments

            StringBuilder getCommentsLink = new StringBuilder("http://comment-microservice-api:8080/api/Comment/GetCommentsByIds?");
            foreach (var commentId in user.CommentsIds)
            {
                getCommentsLink.Append($"ids[]={commentId}&");
            }

            getCommentsLink.Remove(getCommentsLink.Length - 1, 1);
            var createdCommentsResponse = await httpClient.GetAsync(getCommentsLink.ToString());
            if (!createdCommentsResponse.IsSuccessStatusCode) return View("ActionError");

            var comments = await createdCommentsResponse.Content.ReadFromJsonAsync<List<CommentResponse>>();
            ViewBag.Comments = comments;

            #endregion

            #region GettingUserRoles

            var userRolesResponse = await httpClient.GetAsync($"http://register-microservice-api:8080/api/User/GetUserRolesByUserId/{user.AspNetUserId}");
            if (!userRolesResponse.IsSuccessStatusCode) return View("ActionError");
            var userRoles = await userRolesResponse.Content.ReadFromJsonAsync<List<string>>();
            if (userRoles.Contains(UserRoleConstants.AdminRole))
                ViewBag.UserRole = UserRoleConstants.AdminRole;
            else if (userRoles.Contains(UserRoleConstants.ModeratorRole))
                ViewBag.UserRole = UserRoleConstants.ModeratorRole;
            else
            {
                ViewBag.UserRole = UserRoleConstants.UserRole;
            }

            #endregion

            if (User.Identity.Name == user.UserName)
            {
                #region GettingSuggestedComments

                StringBuilder getSuggestedCommentsLink
                    = new StringBuilder("http://comment-microservice-api:8080/api/SuggestComment/GetSuggestedCommentsByIds?");
                foreach (var suggestedCommentId in user.SuggestedCommentsIds)
                {
                    getSuggestedCommentsLink.Append($"ids[]={suggestedCommentId}&");
                }

                getSuggestedCommentsLink.Remove(getSuggestedCommentsLink.Length - 1, 1);
                var suggestedCommentsResponse = await httpClient.GetAsync(getSuggestedCommentsLink.ToString());
                if (!suggestedCommentsResponse.IsSuccessStatusCode) return View("ActionError");

                var suggestedComments = await suggestedCommentsResponse.Content.ReadFromJsonAsync<List<SuggestedCommentResponse>>();
                ViewBag.SuggestedComments = suggestedComments;

                #endregion

                #region GettingSuggestedDiscussions

                StringBuilder getSuggestedDiscussionLink = 
                    new StringBuilder("http://discussion-microservice-api:8080/api/SuggestDiscussion/GetSuggestedDiscussionsByIds?");
                foreach (var suggestedDiscussionId in user.SuggestedDiscussionsIds)
                {
                    getSuggestedDiscussionLink.Append($"ids[]={suggestedDiscussionId}&");
                }

                getSuggestedDiscussionLink.Remove(getSuggestedDiscussionLink.Length - 1, 1);
                var suggestedDiscussionsResponse = await httpClient.GetAsync(getSuggestedDiscussionLink.ToString());
                if (!suggestedDiscussionsResponse.IsSuccessStatusCode) return View("ActionError");

                var suggestedDiscussions = await suggestedDiscussionsResponse.Content.ReadFromJsonAsync<List<SuggestedDiscussionResponse>>();
                ViewBag.SuggestedDiscussions = suggestedDiscussions;

                #endregion

                #region GettingSuggestedTopics

                var getSuggestedTopicResponse = await httpClient.GetAsync(
                    $"http://topic-microservice-api:8080/api/SuggestTopic/GetSuggestedTopicsByUserName/{user.UserName}");
                if (!getSuggestedTopicResponse.IsSuccessStatusCode) return View("ActionError");

                var suggestedTopics = await getSuggestedTopicResponse.Content.ReadFromJsonAsync<List<TopicResponse>>();
                ViewBag.SuggestedTopics = suggestedTopics;

                #endregion
            }
            return View(user);
        }

        [Route("users/{userName}/created/discussions")]
        [HttpGet]
        public async Task<IActionResult> GetUsersCreatedDiscussions(string userName, int pageSize, int pageNumber)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/GetDiscussionsByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var discussions = await response.Content.ReadFromJsonAsync<List<DiscussionResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/Discussion/DoesNextDiscussionsByUserNamePageExist/{userName}?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetDiscussionsViewModel
            {
                PageSize = pageSize, CurrentPageNumber = pageNumber, Discussions = discussions, DoesNextPageExist = doesExist,
                PreviousPageNumber = pageNumber - 1, NextPageNumber = pageNumber + 1
            });
        }

        [Route("users/{userName}/created/comments")]
        [HttpGet]
        public async Task<IActionResult> GetUsersCreatedComments(string userName, int pageSize, int pageNumber)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://comment-microservice-api:8080/api/Comment/GetCommentsByUserName/{userName}?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var comments = await response.Content.ReadFromJsonAsync<List<CommentResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"http://comment-microservice-api:8080/api/Comment/DoesNextCommentsByUserNamePageExist/{userName}?pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetCommentsViewModel
            {
                PageSize = pageSize, DoesNextPageExist = doesExist, CurrentPageNumber = pageNumber, NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1, Comments = comments
            });
        }

        [Route("users/{userName}/suggested/discussions")]
        [HttpGet]
        public async Task<IActionResult> GetUsersSuggestedDiscussions(string userName, int pageSize, int pageNumber)
        {
            return View();
        }

        [Route("users/{userName}/suggested/comments")]
        [HttpGet]
        public async Task<IActionResult> GetUsersSuggestedComments(string userName, int pageSize, int pageNumber)
        {
            return View();
        }

        [Route("users/{userName}/suggested/topics")]
        [HttpGet]
        public async Task<IActionResult> GetUsersSuggestedTopics(string userName, int pageSize, int pageNumber)
        {
            return View();
        }

        [Authorize]
        [Route("[controller]/ChangeUserName")]
        [HttpGet]
        public IActionResult ChangeUserName(string? returnUrl, Guid userId)
        {
            return View(new ChangeUserNameDto{ReturnUrl = returnUrl, UserId = userId});
        }

        [Authorize]
        [Route("[controller]/ChangeUserName")]
        [HttpPost]
        public async Task<IActionResult> ChangeUserName(ChangeUserNameDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient httpClient = httpClientFactory.CreateClient();

                var existsResponse = await httpClient.GetAsync(
                    $"http://user-microservice-api:8080/api/profile/User/IsNormalizedUserNameAlreadyExists/{model.NewUserName}");
                if (!existsResponse.IsSuccessStatusCode) return View("ActionError");
                bool exists = await existsResponse.Content.ReadFromJsonAsync<bool>();
                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "Имя уже занято");
                    return View(model);
                }

                using StringContent jsonContent =
                    new(JsonSerializer.Serialize(new { model.NewUserName, model.UserId }), Encoding.UTF8, "application/json");

                var response = await httpClient.PatchAsync(
                    $"http://user-microservice-api:8080/api/profile/User/ChangeUserName", jsonContent);
                if (!response.IsSuccessStatusCode) return View("ActionError");

                var authResponse = await httpClient.GetAsync(
                    $"http://register-microservice-api:8080/api/User/AuthUserByUserName?userName={model.NewUserName}");
                if (!authResponse.IsSuccessStatusCode) return View("ActionError");

                var authenticated = await authResponse.Content.ReadFromJsonAsync<AuthenticatedResponse>();
                AuthenticateUser(authenticated!.Token);

                if (!string.IsNullOrEmpty(model.ReturnUrl))
                    return LocalRedirect(model.ReturnUrl);

                return LocalRedirect($"/User/{model.NewUserName}");
            }
            return View(model);
        }

        [Authorize]
        [Route("[controller]/ChangePassword")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [Route("[controller]/ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model, Guid userId, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                model.UserId = userId;
                using HttpClient httpClient = httpClientFactory.CreateClient();
                using StringContent jsonContent =
                    new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await httpClient.PatchAsync(
                    "http://register-microservice-api:8080/api/User/ChangePassword", jsonContent);
                if (response.StatusCode == HttpStatusCode.NotFound) return View("ActionError");
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ModelState.AddModelError(string.Empty, "Старый пароль неверный");
                    return View(model);
                }
                if (!response.IsSuccessStatusCode) return View("ActionError");

                if (!string.IsNullOrEmpty(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        private void AuthenticateUser(string jwtToken)
        {
            Request.Cookies.TryGetValue("accessToken", out string? isCookiesExist);
            if(isCookiesExist is not null) Response.Cookies.Delete("accessToken");
            Response.Cookies.Append("accessToken", jwtToken, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMonths(2),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true
            });
        }
    }
}
