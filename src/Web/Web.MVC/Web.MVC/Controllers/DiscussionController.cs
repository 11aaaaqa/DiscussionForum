using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Comment;
using Web.MVC.DTOs.Discussion;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ApiResponses.CommentsResponses;

namespace Web.MVC.Controllers
{
    public class DiscussionController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public DiscussionController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
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
                using StringContent jsonContent = new
                    (JsonSerializer.Serialize(new { model.Title, model.Content, CreatedBy = User.Identity.Name}), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(
                    $"http://discussion-microservice-api:8080/api/SuggestDiscussion/SuggestToCreate?topicName={model.TopicName}",
                    jsonContent);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return View("Thanks");
                }

                return View("ActionError");
            }

            return View(model);
        }

        [HttpGet]
        [Route("discussions/{id}")]
        public async Task<IActionResult> GetDiscussion(Guid id)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response =
                await httpClient.GetAsync(
                    $"http://discussion-microservice-api:8080/api/Discussion/GetDiscussionById?id={id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var discussion = await response.Content.ReadFromJsonAsync<DiscussionResponse>();
                ViewBag.Content = discussion.Content; ViewBag.CreatedAt = discussion.CreatedAt; ViewBag.CreatedBy = discussion.CreatedBy;
                ViewBag.Id = discussion.Id; ViewBag.Rating = discussion.Rating; ViewBag.Title = discussion.Title;
                ViewBag.TopicName = discussion.TopicName; ViewBag.DiscussionId = id; ViewBag.ReturnUrl = HttpContext.Request.Path;

                var getCommentsResponse = await httpClient.GetAsync(
                    $"http://comment-microservice-api:8080/api/Comment/GetCommentsByDiscussionId/{id}");
                if (getCommentsResponse.IsSuccessStatusCode)
                {
                    var comments = await getCommentsResponse.Content.ReadFromJsonAsync<List<CommentResponse>>();
                    ViewBag.Comments = comments;
                }

                return View();
            }

            return View("ActionError");
        }

        [Authorize]
        [HttpPost]
        [Route("discussions/{id}")]
        public async Task<IActionResult> SuggestComment(SuggestCommentDto model, Guid id)
        {
            if (ModelState.IsValid)
            {
                var discussionId = id;
                model.CreatedBy = User.Identity.Name;
                using var httpClient = httpClientFactory.CreateClient();
                using StringContent jsonContent = new(JsonSerializer.Serialize(new
                {
                    model.CreatedBy,
                    model.Content,
                    DiscussionId = discussionId
                }), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://comment-microservice-api:8080/api/SuggestComment/Suggest", jsonContent);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return View("ThanksForComment", discussionId);
                }
                ModelState.AddModelError(string.Empty, "Что-то пошло не так, попробуйте еще раз");
                return View("SomethingWentWrong", discussionId);
            }
            return View("SomethingWentWrong", id);
        }
    }
}
