using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Models.ApiResponses.Bookmark;
using Web.MVC.Models.ViewModels.Bookmark;

namespace Web.MVC.Controllers
{
    public class BookmarkController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public BookmarkController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Route("users/{userName}/bookmarks")]
        [HttpGet]
        public async Task<IActionResult> GetBookmarks(string userName, int pageSize, int pageNumber, string? searchingQuery)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            if (searchingQuery is null)
            {
                var response = await httpClient.GetAsync(
                    $"http://bookmark-microservice-api:8080/api/Bookmark/GetBookmarksByUserName/{userName}?pageSize={pageSize}&pageNumber={pageNumber}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                var bookmarks = await response.Content.ReadFromJsonAsync<List<BookmarkResponseModel>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"http://bookmark-microservice-api:8080/api/Bookmark/DoesNextBookmarksByUserNamePageExist/{userName}?pageSize={pageSize}&pageNumber={pageNumber  + 1}");
                if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

                bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
                return View(new BookmarkViewModel
                {
                    PageSize = pageSize, DoesNextPageExist = doesExist, CurrentPageNumber = pageNumber, NextPageNumber = pageNumber + 1,
                    PreviousPageNumber = pageNumber - 1, UserName = userName, Bookmarks = bookmarks, SearchingQuery = searchingQuery
                });
            }

            var findBookmarksResponse = await httpClient.GetAsync(
                $"http://bookmark-microservice-api:8080/api/Bookmark/FindBookmarks/{userName}?pageSize={pageSize}&pageNumber={pageNumber}&searchingQuery={searchingQuery}");
            if (!findBookmarksResponse.IsSuccessStatusCode) return View("ActionError");

            var foundBookmarks = await findBookmarksResponse.Content.ReadFromJsonAsync<List<BookmarkResponseModel>>();

            var doesNextFindPageExistResponse = await httpClient.GetAsync(
                $"http://bookmark-microservice-api:8080/api/Bookmark/DoesNextFindBookmarksPageExist/{userName}?pageSize={pageSize}&pageNumber={pageNumber + 1}&searchingQuery={searchingQuery}");
            if (!doesNextFindPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesNextFindPageExist = await doesNextFindPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new BookmarkViewModel
            {
                PageSize = pageSize, CurrentPageNumber = pageNumber, DoesNextPageExist = doesNextFindPageExist, NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1, UserName = userName, Bookmarks = foundBookmarks, SearchingQuery = searchingQuery
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBookmark(Guid discussionId, string discussionTitle, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                UserName = User.Identity.Name, DiscussionId = discussionId, DiscussionTitle = discussionTitle
            }), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                $"http://bookmark-microservice-api:8080/api/Bookmark/AddBookmark", jsonContent);
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
