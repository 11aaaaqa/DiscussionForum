using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;
using Web.MVC.Models.ApiResponses.Discussion;

namespace Web.MVC.Controllers
{
    public class SuggestDiscussion : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;

        public SuggestDiscussion(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            url = (string.IsNullOrEmpty(config["Url:Port"]))
                ? $"{config["Url:Protocol"]}://{config["Url:HostName"]}" : $"{config["Url:Protocol"]}://{config["Url:HostName"]}:{config["Url:Port"]}";
        }

        [Authorize(Roles = UserRoleConstants.AdminRole + ", " + UserRoleConstants.ModeratorRole)]
        [HttpPost]
        public async Task<IActionResult> DeleteSuggestedDiscussion(Guid suggestedDiscussionId, string returnUrl)
        {
            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"{url}/api/SuggestDiscussion/RejectSuggestedDiscussion/{suggestedDiscussionId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = UserRoleConstants.AdminRole + ", " + UserRoleConstants.ModeratorRole)]
        [Route("MySuggestions/{suggestedDiscussionId}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedDiscussionById(Guid suggestedDiscussionId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"{url}/api/SuggestDiscussion/GetSuggestedDiscussionById?id={suggestedDiscussionId}");

            if (!response.IsSuccessStatusCode) return View("ActionError");

            var suggestedComments = await response.Content.ReadFromJsonAsync<SuggestedDiscussionResponse>();
            return View(suggestedComments);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteMySuggestedDiscussion(Guid suggestedDiscussionId, string returnUrl)
        {
            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"{url}/api/SuggestDiscussion/RejectSuggestedDiscussion/{suggestedDiscussionId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
