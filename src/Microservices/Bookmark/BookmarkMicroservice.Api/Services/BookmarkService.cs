using BookmarkMicroservice.Api.Database;
using BookmarkMicroservice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookmarkMicroservice.Api.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly ApplicationDbContext context;

        public BookmarkService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<List<Bookmark>> GetAllBookmarks(BookmarkParameters bookmarkParameters)
        {
            var bookmarks = await context.Bookmarks
                .Skip(bookmarkParameters.PageSize * (bookmarkParameters.PageNumber - 1))
                .Take(bookmarkParameters.PageSize).ToListAsync();
            return bookmarks;
        }

        public async Task<List<Bookmark>> FindBookmarks(string searchingString, BookmarkParameters bookmarkParameters)
        {
            var bookmarks = await context.Bookmarks.Where(x => x.DiscussionTitle.ToLower().Contains(searchingString.ToLower()))
                .Skip(bookmarkParameters.PageSize * (bookmarkParameters.PageNumber - 1))
                .Take(bookmarkParameters.PageSize).ToListAsync();
            return bookmarks;
        }

        public async Task<Bookmark> AddBookmark(Bookmark bookmark)
        {
            await context.Bookmarks.AddAsync(bookmark);
            await context.SaveChangesAsync();
            return bookmark;
        }

        public async Task DeleteBookmark(Guid bookmarkId)
        {
            context.Bookmarks.Remove(new Bookmark { Id = bookmarkId });
            await context.SaveChangesAsync();
        }
    }
}
