using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;

namespace Web.MVC.Controllers
{
    public class SuggestComment : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;

        public SuggestComment(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            url = (string.IsNullOrEmpty(config["Url:Port"]))
                ? $"{config["Url:Protocol"]}://{config["Url:HostName"]}" : $"{config["Url:Protocol"]}://{config["Url:HostName"]}:{config["Url:Port"]}";
        }

        [Authorize(Roles = UserRoleConstants.AdminRole + ", " + UserRoleConstants.ModeratorRole)]
        [HttpPost]
        public async Task<IActionResult> DeleteSuggestedComment(Guid suggestedCommentId, string returnUrl)
        {
            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"{url}/api/SuggestComment/RejectSuggestedComment/{suggestedCommentId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
