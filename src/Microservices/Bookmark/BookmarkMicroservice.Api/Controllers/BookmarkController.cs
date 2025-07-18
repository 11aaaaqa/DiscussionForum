﻿using BookmarkMicroservice.Api.DTOs;
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

        [Route("GetBookmarksByUserNameByNovelty/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetBookmarksByUserNameByNoveltyAsync(string userName, [FromQuery] BookmarkParameters bookmarkParameters)
        {
            var bookmarks = await bookmarkService.GetBookmarksByUserNameByNovelty(userName, bookmarkParameters);
            return Ok(bookmarks);
        }

        [Route("GetBookmarksByUserNameByAntiquity/{userName}")]
        [HttpGet]
        public async Task<IActionResult> GetBookmarksByUserNameByAntiquityAsync(string userName, [FromQuery] BookmarkParameters bookmarkParameters)
        {
            var bookmarks = await bookmarkService.GetBookmarksByUserNameByAntiquity(userName, bookmarkParameters);
            return Ok(bookmarks);
        }

        [Route("DoesNextBookmarksByUserNamePageExist/{userName}")]
        [HttpGet]
        public async Task<IActionResult> DoesNextBookmarksByUserNamePageExistAsync(string userName, [FromQuery] BookmarkParameters bookmarkParameters)
        {
            bool doesExist = await paginationService.DoesNextBookmarksByUserNamePageExist(userName, bookmarkParameters);
            return Ok(doesExist);
        }

        [Route("FindBookmarksByNovelty/{userName}")]
        [HttpGet]
        public async Task<IActionResult> FindBookmarksByNoveltyAsync(string userName, string searchingQuery,
            [FromQuery] BookmarkParameters bookmarkParameters)
        {
            var bookmarks = await bookmarkService.FindBookmarksByNovelty(userName, searchingQuery, bookmarkParameters);
            return Ok(bookmarks);
        }

        [Route("FindBookmarksByAntiquity/{userName}")]
        [HttpGet]
        public async Task<IActionResult> FindBookmarksByAntiquityAsync(string userName, string searchingQuery,
            [FromQuery] BookmarkParameters bookmarkParameters)
        {
            var bookmarks = await bookmarkService.FindBookmarksByAntiquity(userName, searchingQuery, bookmarkParameters);
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
            Bookmark createdBookmark;
            try
            {
                createdBookmark = await bookmarkService.AddBookmark(new Bookmark
                {
                    DiscussionId = model.DiscussionId, DiscussionTitle = model.DiscussionTitle, Id = Guid.NewGuid(), UserName = model.UserName,
                    AddedAt = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                return Conflict();
            }
            
            return Ok(createdBookmark);
        }

        [Route("DeleteBookmark/{bookmarkId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBookmarkAsync(Guid bookmarkId)
        {
            await bookmarkService.DeleteBookmark(bookmarkId);
            return Ok();
        }

        [Route("IsInBookmarks")]
        [HttpGet]
        public async Task<IActionResult> IsInBookmarksAsync([FromQuery] Guid discussionId, [FromQuery] string userName)
        {
            var isInBookmarks = await bookmarkService.IsInBookmarks(discussionId, userName);
            return Ok(isInBookmarks);
        }
    }
}
