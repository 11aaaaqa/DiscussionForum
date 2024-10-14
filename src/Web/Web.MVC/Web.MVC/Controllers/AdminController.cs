using Microsoft.AspNetCore.Mvc;
using Web.MVC.Models.ApiResponses;
using Web.MVC.Models.ViewModels.AdminViewModels;

namespace Web.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Route("admin/history/bans")]
        [HttpGet]
        public async Task<IActionResult> GetBansHistory(string? searchingQuery, int pageSize, int pageNumber)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            if (searchingQuery is null)
            {
                var response = await httpClient.GetAsync(
                    $"http://banhistory-microservice-api:8080/api/BanHistory/GetAllBans?pageSize={pageSize}&pageNumber={pageNumber}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                var bans = await response.Content.ReadFromJsonAsync<List<BanHistoryModel>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"http://banhistory-microservice-api:8080/api/BanHistory/DoesNextAllBansPageExist?pageSize={pageSize}&pageNumber={pageNumber + 1}");
                if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

                bool doesNextPageExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
                return View(new GetBansHistoryViewModel
                {
                    DoesNextPageExist = doesNextPageExist, CurrentPageNumber = pageNumber, NextPageNumber = pageNumber + 1,
                    PreviousPageNumber = pageNumber - 1, Bans = bans, PageSize = pageSize, SearchingQuery = null
                });
            }

            var searchingBansResponse = await httpClient.GetAsync(
                $"http://banhistory-microservice-api:8080/api/BanHistory/FindBans?searchingString={searchingQuery}&pageSize={pageSize}&pageNumber={pageNumber}");
            if (!searchingBansResponse.IsSuccessStatusCode) return View("ActionError");

            var foundBans = await searchingBansResponse.Content.ReadFromJsonAsync<List<BanHistoryModel>>();

            var doesNextPageSearchingExistResponse = await httpClient.GetAsync(
                $"http://banhistory-microservice-api:8080/api/BanHistory/DoesNextFindBansPageExist?searchingString={searchingQuery}&pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextPageSearchingExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageSearchingExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetBansHistoryViewModel
            {
                CurrentPageNumber = pageNumber, DoesNextPageExist = doesExist, NextPageNumber = pageNumber + 1, PreviousPageNumber = pageNumber - 1,
                Bans = foundBans, PageSize = pageSize, SearchingQuery = searchingQuery
            });
        }
    }
}
