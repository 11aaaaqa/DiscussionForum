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

        [Route("GetBookmarksByUserId/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetBookmarksByUserIdAsync(Guid userId, [FromQuery] BookmarkParameters bookmarkParameters)
        {
            var bookmarks = await bookmarkService.GetBookmarksByUserId(userId, bookmarkParameters);
            return Ok(bookmarks);
        }

        [Route("DoesNextBookmarksByIdPageExist/{userId}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextBookmarksByIdPageExistAsync(Guid userId, [FromQuery] BookmarkParameters bookmarkParameters)
        {
            bool doesExist = await paginationService.DoesNextBookmarksByUserIdPageExist(userId, bookmarkParameters);
            return Ok(doesExist);
        }

        [Route("FindBookmarks/{userId}")]
        [HttpGet]
        public async Task<IActionResult> FindBookmarksAsync(Guid userId, string searchingQuery, [FromQuery] BookmarkParameters bookmarkParameters)
        {
            var bookmarks = await bookmarkService.FindBookmarks(userId, searchingQuery, bookmarkParameters);
            return Ok(bookmarks);
        }

        [Route("DoesNextFindBookmarksPageExist/{userId}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextFindBookmarksPageExistAsync(Guid userId, [FromQuery] BookmarkParameters bookmarkParameters,
            string searchingQuery)
        {
            bool doesExist = await paginationService.DoesNextFindBookmarksPageExist(userId, searchingQuery, bookmarkParameters);
            return Ok(doesExist);
        }

        [Route("AddBookmark")]
        [HttpPost]
        public async Task<IActionResult> AddBookmarkAsync([FromBody] BookmarkDto model)
        {
            var createdBookmark = await bookmarkService.AddBookmark(new Bookmark
            {
                DiscussionId = model.DiscussionId, DiscussionTitle = model.DiscussionTitle, Id = Guid.NewGuid(), UserId = model.UserId
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
