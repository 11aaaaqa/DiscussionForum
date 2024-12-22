using BookmarkMicroservice.Api.DTOs;
using BookmarkMicroservice.Api.Models;

namespace BookmarkMicroservice.Api.Services
{
    public interface IBookmarkService
    {
        Task<List<Bookmark>> GetBookmarksByUserNameByNovelty(string userName, BookmarkParameters bookmarkParameters);
        Task<List<Bookmark>> GetBookmarksByUserNameByAntiquity(string userName, BookmarkParameters bookmarkParameters);
        Task<List<Bookmark>> FindBookmarksByNovelty(string userName, string searchingString, BookmarkParameters bookmarkParameters);
        Task<List<Bookmark>> FindBookmarksByAntiquity(string userName, string searchingString, BookmarkParameters bookmarkParameters);
        Task<Bookmark> AddBookmark(Bookmark bookmark);
        Task DeleteBookmark(Guid bookmarkId);
        Task<IsInBookmarksDto> IsInBookmarks(Guid discussionId, string userName);
    }
}
