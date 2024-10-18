using BookmarkMicroservice.Api.DTOs;
using BookmarkMicroservice.Api.Models;
using BookmarkMicroservice.Api.Services;
using BookmarkMicroservice.Api.Services.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkService bookmarkService;
        private readonly IPaginationService paginationService;

        public BookmarkController(IBookmarkService bookmarkService, IPaginationService paginationService)
        {
            this.paginationService = paginationService;
            this.bookmarkService = bookmarkService;
        }

        [Route("GetBookmarksByUserName/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetBookmarksByUserNameAsync(string userName, [FromQuery] BookmarkParameters bookmarkParameters)
        {
            var bookmarks = await bookmarkService.GetBookmarksByUserName(userName, bookmarkParameters);
            return Ok(bookmarks);
        }

        [Route("DoesNextBookmarksByUserNamePageExist/{userName}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextBookmarksByUserNamePageExistAsync(string userName, [FromQuery] BookmarkParameters bookmarkParameters)
        {
            bool doesExist = await paginationService.DoesNextBookmarksByUserNamePageExist(userName, bookmarkParameters);
            return Ok(doesExist);
        }

        [Route("FindBookmarks/{userName}")]
        [HttpGet]
        public async Task<IActionResult> FindBookmarksAsync(string userName, string searchingQuery, [FromQuery] BookmarkParameters bookmarkParameters)
        {
            var bookmarks = await bookmarkService.FindBookmarks(userName, searchingQuery, bookmarkParameters);
            return Ok(bookmarks);
        }

        [Route("DoesNextFindBookmarksPageExist/{userName}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextFindBookmarksPageExistAsync(string userName, [FromQuery] BookmarkParameters bookmarkParameters,
            string searchingQuery)
        {
            bool doesExist = await paginationService.DoesNextFindBookmarksPageExist(userName, searchingQuery, bookmarkParameters);
            return Ok(doesExist);
        }

        [Route("AddBookmark")]
        [HttpPost]
        public async Task<IActionResult> AddBookmarkAsync([FromBody] BookmarkDto model)
        {
            var createdBookmark = await bookmarkService.AddBookmark(new Bookmark
            {
                DiscussionId = model.DiscussionId, DiscussionTitle = model.DiscussionTitle, Id = Guid.NewGuid(), UserName = model.UserName
            });
            return Ok(createdBookmark);
        }

        [Route("DeleteBookmark/{bookmarkId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBookmarkAsync(Guid bookmarkId)
        {
            await bookmarkService.DeleteBookmark(bookmarkId);
            return Ok();
        }
    }
}
