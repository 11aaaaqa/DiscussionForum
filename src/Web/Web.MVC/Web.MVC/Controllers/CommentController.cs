using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.MVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CommentController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteComment(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"http://comment-microservice-api:8080/api/Comment/DeleteCommentById/{id}");
            if (response.IsSuccessStatusCode)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                    return LocalRedirect(returnUrl);
                return RedirectToAction("Topics", "Topic");
            }

            return View("ActionError");
        }
    }
}
