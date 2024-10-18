using BookmarkMicroservice.Api.Models;

namespace BookmarkMicroservice.Api.Services
{
    public interface IBookmarkService
    {
        Task<List<Bookmark>> GetBookmarksByUserName(string userName, BookmarkParameters bookmarkParameters);
        Task<List<Bookmark>> FindBookmarks(string userName, string searchingString, BookmarkParameters bookmarkParameters);
        Task<Bookmark> AddBookmark(Bookmark bookmark);
        Task DeleteBookmark(Guid bookmarkId);
    }
}
