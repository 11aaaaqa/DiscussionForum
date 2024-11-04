using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;
using Web.MVC.Models.ApiResponses.Bookmark;
using Web.MVC.Models.ViewModels.Bookmark;

namespace Web.MVC.Controllers
{
    public class BookmarkController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;

        public BookmarkController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            url = (string.IsNullOrEmpty(config["Url:Port"]))
                ? $"{config["Url:Protocol"]}://{config["Url:HostName"]}" : $"{config["Url:Protocol"]}://{config["Url:HostName"]}:{config["Url:Port"]}";
        }

        [Authorize]
        [Route("users/{userName}/bookmarks/oldest")]
        [HttpGet]
        public async Task<IActionResult> GetBookmarksByAntiquity(string userName, int pageSize, int pageNumber, string? searchingQuery)
        {
            if (User.Identity.Name != userName && !User.IsInRole(UserRoleConstants.AdminRole) && !User.IsInRole(UserRoleConstants.ModeratorRole))
                return RedirectToAction("AccessIsForbidden", "Information");

            using HttpClient httpClient = httpClientFactory.CreateClient();

            if (searchingQuery is null)
            {
                var response = await httpClient.GetAsync(
                    $"{url}/api/Bookmark/GetBookmarksByUserNameByAntiquity/{userName}?pageSize={pageSize}&pageNumber={pageNumber}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                var bookmarks = await response.Content.ReadFromJsonAsync<List<BookmarkResponseModel>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"{url}/api/Bookmark/DoesNextBookmarksByUserNamePageExist/{userName}?pageSize={pageSize}&pageNumber={pageNumber  + 1}");
                if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

                bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
                return View(new BookmarkViewModel
                {
                    PageSize = pageSize, DoesNextPageExist = doesExist, CurrentPageNumber = pageNumber, NextPageNumber = pageNumber + 1,
                    PreviousPageNumber = pageNumber - 1, UserName = userName, Bookmarks = bookmarks, SearchingQuery = searchingQuery
                });
            }

            var findBookmarksResponse = await httpClient.GetAsync(
                $"{url}/api/Bookmark/FindBookmarksByAntiquity/{userName}?pageSize={pageSize}&pageNumber={pageNumber}&searchingQuery={searchingQuery}");
            if (!findBookmarksResponse.IsSuccessStatusCode) return View("ActionError");

            var foundBookmarks = await findBookmarksResponse.Content.ReadFromJsonAsync<List<BookmarkResponseModel>>();

            var doesNextFindPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/Bookmark/DoesNextFindBookmarksPageExist/{userName}?pageSize={pageSize}&pageNumber={pageNumber + 1}&searchingQuery={searchingQuery}");
            if (!doesNextFindPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesNextFindPageExist = await doesNextFindPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new BookmarkViewModel
            {
                PageSize = pageSize, CurrentPageNumber = pageNumber, DoesNextPageExist = doesNextFindPageExist, NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1, UserName = userName, Bookmarks = foundBookmarks, SearchingQuery = searchingQuery
            });
        }

        [Authorize]
        [Route("users/{userName}/bookmarks/latest")]
        [HttpGet]
        public async Task<IActionResult> GetBookmarksByNovelty(string userName, int pageSize, int pageNumber, string? searchingQuery)
        {
            if (User.Identity.Name != userName && !User.IsInRole(UserRoleConstants.AdminRole) && !User.IsInRole(UserRoleConstants.ModeratorRole))
                return RedirectToAction("AccessIsForbidden", "Information");

            using HttpClient httpClient = httpClientFactory.CreateClient();

            if (searchingQuery is null)
            {
                var response = await httpClient.GetAsync(
                    $"{url}/api/Bookmark/GetBookmarksByUserNameByNovelty/{userName}?pageSize={pageSize}&pageNumber={pageNumber}");
                if (!response.IsSuccessStatusCode) return View("ActionError");

                var bookmarks = await response.Content.ReadFromJsonAsync<List<BookmarkResponseModel>>();

                var doesNextPageExistResponse = await httpClient.GetAsync(
                    $"{url}/api/Bookmark/DoesNextBookmarksByUserNamePageExist/{userName}?pageSize={pageSize}&pageNumber={pageNumber + 1}");
                if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

                bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();
                return View(new BookmarkViewModel
                {
                    PageSize = pageSize,
                    DoesNextPageExist = doesExist,
                    CurrentPageNumber = pageNumber,
                    NextPageNumber = pageNumber + 1,
                    PreviousPageNumber = pageNumber - 1,
                    UserName = userName,
                    Bookmarks = bookmarks,
                    SearchingQuery = searchingQuery
                });
            }

            var findBookmarksResponse = await httpClient.GetAsync(
                $"{url}/api/Bookmark/FindBookmarksByNovelty/{userName}?pageSize={pageSize}&pageNumber={pageNumber}&searchingQuery={searchingQuery}");
            if (!findBookmarksResponse.IsSuccessStatusCode) return View("ActionError");

            var foundBookmarks = await findBookmarksResponse.Content.ReadFromJsonAsync<List<BookmarkResponseModel>>();

            var doesNextFindPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/Bookmark/DoesNextFindBookmarksPageExist/{userName}?pageSize={pageSize}&pageNumber={pageNumber + 1}&searchingQuery={searchingQuery}");
            if (!doesNextFindPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            bool doesNextFindPageExist = await doesNextFindPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new BookmarkViewModel
            {
                PageSize = pageSize,
                CurrentPageNumber = pageNumber,
                DoesNextPageExist = doesNextFindPageExist,
                NextPageNumber = pageNumber + 1,
                PreviousPageNumber = pageNumber - 1,
                UserName = userName,
                Bookmarks = foundBookmarks,
                SearchingQuery = searchingQuery
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
                $"{url}/api/Bookmark/AddBookmark", jsonContent);
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteBookmark(Guid bookmarkId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.DeleteAsync(
                $"{url}/api/Bookmark/DeleteBookmark/{bookmarkId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
