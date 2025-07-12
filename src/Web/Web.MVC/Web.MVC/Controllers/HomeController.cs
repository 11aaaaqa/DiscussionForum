using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Web.MVC.Models.ApiResponses.Discussion;
using Web.MVC.Models;

namespace Web.MVC.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Counter HomePageVisitingCounter = Metrics.CreateCounter
        ("home_page_visiting", "Number of times visited home page.");

        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            url = (string.IsNullOrEmpty(config["Url:Port"]))
                ? $"{config["Url:Protocol"]}://{config["Url:HostName"]}" : $"{config["Url:Protocol"]}://{config["Url:HostName"]}:{config["Url:Port"]}";
        }
        
        public async Task<IActionResult> Index()
        {
            HomePageVisitingCounter.Inc();
            byte pageSize = 20;
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"{url}/api/Discussion/GetAllDiscussionsSortedByPopularityForWeek?pageSize={pageSize}&pageNumber=1");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var discussions = await response.Content.ReadFromJsonAsync<List<DiscussionResponse>>();
            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/Discussion/DoesNextDiscussionsForTodayPageExist?pageSize={pageSize}&pageNumber=2");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetDiscussionsViewModel
            {
                PageSize = pageSize,
                CurrentPageNumber = 1,
                DoesNextPageExist = doesExist,
                NextPageNumber = 2,
                PreviousPageNumber = 0,
                Discussions = discussions
            });
        }
    }
}
