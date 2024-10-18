using BookmarkMicroservice.Api.Models;

namespace BookmarkMicroservice.Api.Services.Pagination
{
    public interface IPaginationService
    {
        Task<bool> DoesNextBookmarksByUserIdPageExist(Guid userId, BookmarkParameters  bookmarkParameters);
        Task<bool> DoesNextFindBookmarksPageExist(Guid userId, string searchingString, BookmarkParameters bookmarkParameters);
    }
}
