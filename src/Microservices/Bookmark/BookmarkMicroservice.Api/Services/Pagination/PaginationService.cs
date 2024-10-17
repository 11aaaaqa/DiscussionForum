using BookmarkMicroservice.Api.Database;
using BookmarkMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookmarkMicroservice.Api.Services.Pagination
{
    public class PaginationService : IPaginationService
    {
        private readonly ApplicationDbContext context;

        public PaginationService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> DoesNextAllBookmarksPageExist(BookmarkParameters bookmarkParameters)
        {
            int totalBookmarksCount = await context.Bookmarks.CountAsync();
            int totalRequestedBookmarksCount = bookmarkParameters.PageSize * bookmarkParameters.PageNumber;
            int startedRequestedBookmarksCount = totalRequestedBookmarksCount - bookmarkParameters.PageSize;
            bool doesExist = (totalBookmarksCount > startedRequestedBookmarksCount);
            return doesExist;
        }

        public async Task<bool> DoesNextFindBookmarksPageExist(string searchingString, BookmarkParameters bookmarkParameters)
        {
            int totalBookmarksCount = await context.Bookmarks.Where(x => x.DiscussionTitle.ToLower().Contains(searchingString.ToLower()))
                .CountAsync();
            int totalRequestedBookmarksCount = bookmarkParameters.PageSize * bookmarkParameters.PageNumber;
            int startedRequestedBookmarksCount = totalRequestedBookmarksCount - bookmarkParameters.PageSize;
            bool doesExist = (totalBookmarksCount > startedRequestedBookmarksCount);
            return doesExist;
        }
    }
}
