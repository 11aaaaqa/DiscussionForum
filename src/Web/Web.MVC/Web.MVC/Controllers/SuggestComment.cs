using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;

namespace Web.MVC.Controllers
{
    public class SuggestComment : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public SuggestComment(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Authorize(Roles = UserRoleConstants.AdminRole + ", " + UserRoleConstants.ModeratorRole)]
        [HttpPost]
        public async Task<IActionResult> DeleteSuggestedComment(Guid suggestedCommentId, string returnUrl)
        {
            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://comment-microservice-api:8080/api/SuggestComment/RejectSuggestedComment/{suggestedCommentId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
