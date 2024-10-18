using BookmarkMicroservice.Api.Models;

namespace BookmarkMicroservice.Api.Services
{
    public interface IBookmarkService
    {
        Task<List<Bookmark>> GetBookmarksByUserId(Guid userId, BookmarkParameters bookmarkParameters);
        Task<List<Bookmark>> FindBookmarks(Guid userId, string searchingString, BookmarkParameters bookmarkParameters);
        Task<Bookmark> AddBookmark(Bookmark bookmark);
        Task DeleteBookmark(Guid bookmarkId);
    }
}
