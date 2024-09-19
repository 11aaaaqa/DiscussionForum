using Microsoft.AspNetCore.Mvc;
using Web.MVC.Models.ApiResponses.Discussion;

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

        [Route("MySuggestions/{suggestedDiscussionId}")]
        [HttpGet]
        public async Task<IActionResult> GetSuggestedDiscussionById(Guid suggestedDiscussionId)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://discussion-microservice-api:8080/api/SuggestDiscussion/GetSuggestedDiscussionById?id={suggestedDiscussionId}");

            if (!response.IsSuccessStatusCode) return View("ActionError");

            var suggestedComments = await response.Content.ReadFromJsonAsync<SuggestedDiscussionResponse>();
            return View(suggestedComments);
        }
    }
}
