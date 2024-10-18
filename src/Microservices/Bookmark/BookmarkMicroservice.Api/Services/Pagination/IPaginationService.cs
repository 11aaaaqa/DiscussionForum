using BookmarkMicroservice.Api.Models;

namespace BookmarkMicroservice.Api.Services.Pagination
{
    public interface IPaginationService
    {
        Task<bool> DoesNextBookmarksByUserNamePageExist(string userName, BookmarkParameters  bookmarkParameters);
        Task<bool> DoesNextFindBookmarksPageExist(string userName, string searchingString, BookmarkParameters bookmarkParameters);
    }
}
