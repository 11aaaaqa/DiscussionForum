using System.Text;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Models.ApiResponses.CommentsResponses;
using Web.MVC.Models.ApiResponses.CustUserResponses;
using Web.MVC.Models.ApiResponses.Discussion;

namespace Web.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Route("[controller]/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetUser(string userName)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"http://user-microservice-api:8080/api/profile/User/GetUserByUserName/{userName}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var user = await response.Content.ReadFromJsonAsync<UserResponse>();

            #region GetDiscussions

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


            #region GetComments

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

            return View(user);
        }
    }
}
