using Microsoft.AspNetCore.Mvc;

namespace Web.MVC.Controllers
{
    public class SuggestTopic : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public SuggestTopic(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSuggestedTopicById(Guid id, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync(
                $"http://topic-microservice-api:8080/api/SuggestTopic/RejectSuggestedTopic/{id}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl)) 
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
