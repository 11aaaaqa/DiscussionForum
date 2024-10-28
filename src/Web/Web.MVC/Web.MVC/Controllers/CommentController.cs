using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.MVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;

        public CommentController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            url = (string.IsNullOrEmpty(config["Url:Port"]))
                ? $"{config["Url:Protocol"]}://{config["Url:HostName"]}" : $"{config["Url:Protocol"]}://{config["Url:HostName"]}:{config["Url:Port"]}";
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteComment(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"{url}/api/Comment/DeleteCommentById/{id}");
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
