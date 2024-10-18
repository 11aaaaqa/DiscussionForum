using BookmarkMicroservice.Api.Controllers;
using BookmarkMicroservice.Api.DTOs;
using BookmarkMicroservice.Api.Models;
using BookmarkMicroservice.Api.Services;
using BookmarkMicroservice.Api.Services.Pagination;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookmarkMicroservice.UnitTests
{
    public class BookmarkControllerTests
    {
        [Fact]
        public async Task GetAllBookmarksAsync_ReturnsOkWithListOfBookmarks()
        {
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IBookmarkService>();
            mock.Setup(x => x.GetAllBookmarks(bookmarkParameters)).ReturnsAsync(new List<Bookmark>
            {
                new Bookmark{Id = Guid.NewGuid()}, new Bookmark{Id = Guid.NewGuid()}, new Bookmark{Id = Guid.NewGuid()}
            });
            var controller = new BookmarkController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetAllBookmarksAsync(bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var bookmarks = Assert.IsType<List<Bookmark>>(methodResult.Value);
            Assert.Equal(3, bookmarks.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextAllBookmarksPageExistAsync_ReturnsOkWithTrue()
        {
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextAllBookmarksPageExist(bookmarkParameters)).ReturnsAsync(true);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextAllBookmarksPageExistAsync(bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.True(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextAllBookmarksPageExistAsync_ReturnsOkWithFalse()
        {
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextAllBookmarksPageExist(bookmarkParameters)).ReturnsAsync(false);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextAllBookmarksPageExistAsync(bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.False(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task FindBookmarksAsync_ReturnsOkWithListOfBookmarks()
        {
            string searchingString = "test";
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IBookmarkService>();
            mock.Setup(x => x.FindBookmarks(searchingString, bookmarkParameters)).ReturnsAsync(new List<Bookmark>
            {
                new Bookmark{DiscussionTitle = "test123"}, new Bookmark{DiscussionTitle = "test"}, new Bookmark{DiscussionTitle = "test5"}
            });
            var controller = new BookmarkController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.FindBookmarksAsync(searchingString, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var bookmarks = Assert.IsType<List<Bookmark>>(methodResult.Value);
            Assert.Equal(3, bookmarks.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextFindBookmarksPageExistAsync_ReturnsOkWithFalse()
        {
            string searchingString = "test";
            var parameters = new BookmarkParameters { PageNumber = 2, PageSize = 3};
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextFindBookmarksPageExist(searchingString, parameters)).ReturnsAsync(false);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextFindBookmarksPageExistAsync(parameters, searchingString);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.False(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextFindBookmarksPageExistAsync_ReturnsOkWithTrue()
        {
            string searchingString = "test";
            var parameters = new BookmarkParameters { PageNumber = 2, PageSize = 3 };
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextFindBookmarksPageExist(searchingString, parameters)).ReturnsAsync(true);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextFindBookmarksPageExistAsync(parameters, searchingString);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.True(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task AddBookmarkAsync_ReturnsOkWithCreatedBookmark()
        {
            var bookmarkModel = new BookmarkDto {DiscussionId = Guid.NewGuid(), DiscussionTitle = "tests", UserId = Guid.NewGuid()};
            var mock = new Mock<IBookmarkService>();
            var controller = new BookmarkController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.AddBookmarkAsync(bookmarkModel);

            Assert.IsType<OkObjectResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteBookmarkAsync_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var mock = new Mock<IBookmarkService>();
            mock.Setup(x => x.DeleteBookmark(id));
            var controller = new BookmarkController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.DeleteBookmarkAsync(id);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }
    }
}
