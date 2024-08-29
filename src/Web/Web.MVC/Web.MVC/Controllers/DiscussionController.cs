using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Discussion;

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
    }
}
