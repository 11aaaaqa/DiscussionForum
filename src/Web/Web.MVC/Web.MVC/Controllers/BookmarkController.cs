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
        public async Task<IActionResult> GetBookmarks(string userName, Guid userId, int pageSize, int pageNumber)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://bookmark-microservice-api:8080/api/Bookmark/GetBookmarksByUserId/{userId}?pageSize={pageSize}&pageNumber={pageNumber}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var bookmarks = await response.Content.ReadFromJsonAsync<List<BookmarkResponseModel>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"http://bookmark-microservice-api:8080/api/Bookmark/DoesNextBookmarksByIdPageExist/{userId}?pageNumber={pageNumber + 1}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            bool doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new BookmarkViewModel
            {
                PageSize = pageSize, PreviousPageNumber = pageNumber - 1, CurrentPageNumber = pageNumber, NextPageNumber = pageNumber + 1,
                UserName = userName, UserId = userId, Bookmarks = bookmarks, DoesNextPageExist = doesExist
            });
        }
    }
}
