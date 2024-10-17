using BookmarkMicroservice.Api.Models;

namespace BookmarkMicroservice.Api.Services.Pagination
{
    public interface IPaginationService
    {
        Task<bool> DoesNextAllBookmarksPageExist(BookmarkParameters  bookmarkParameters);
        Task<bool> DoesNextFindBookmarksPageExist(string searchingString, BookmarkParameters bookmarkParameters);
    }
}
