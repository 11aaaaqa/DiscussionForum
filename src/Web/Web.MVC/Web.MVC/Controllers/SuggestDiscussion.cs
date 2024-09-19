using Microsoft.AspNetCore.Mvc;

namespace Web.MVC.Controllers
{
    public class SuggestDiscussion : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public SuggestDiscussion(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSuggestedDiscussion(Guid suggestedDiscussionId, string returnUrl)
        {
            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://discussion-microservice-api:8080/api/SuggestDiscussion/RejectSuggestedDiscussion/{suggestedDiscussionId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
