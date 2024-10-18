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
        public async Task GetBookmarksByUserIdAsync_ReturnsOkWithListOfBookmarks()
        {
            var userId = Guid.NewGuid();
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IBookmarkService>();
            mock.Setup(x => x.GetBookmarksByUserId(userId, bookmarkParameters)).ReturnsAsync(new List<Bookmark>
            {
                new Bookmark{Id = Guid.NewGuid(), UserId = userId}, new Bookmark{Id = Guid.NewGuid(), UserId = userId}, 
                new Bookmark{Id = Guid.NewGuid(), UserId = userId}
            });
            var controller = new BookmarkController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetBookmarksByUserIdAsync(userId, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var bookmarks = Assert.IsType<List<Bookmark>>(methodResult.Value);
            Assert.Equal(3, bookmarks.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextBookmarksByIdPageExistAsync_ReturnsOkWithTrue()
        {
            var userId = Guid.NewGuid();
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextBookmarksByUserIdPageExist(userId, bookmarkParameters)).ReturnsAsync(true);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextBookmarksByIdPageExistAsync(userId, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.True(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextAllBookmarksPageExistAsync_ReturnsOkWithFalse()
        {
            var userId = Guid.NewGuid();
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextBookmarksByUserIdPageExist(userId, bookmarkParameters)).ReturnsAsync(false);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextBookmarksByIdPageExistAsync(userId, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.False(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task FindBookmarksAsync_ReturnsOkWithListOfBookmarks()
        {
            var userId = Guid.NewGuid();
            string searchingString = "test";
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IBookmarkService>();
            mock.Setup(x => x.FindBookmarks(userId, searchingString, bookmarkParameters)).ReturnsAsync(new List<Bookmark>
            {
                new Bookmark{DiscussionTitle = "test123", UserId = userId},
                new Bookmark{DiscussionTitle = "test", UserId = userId},
                new Bookmark{DiscussionTitle = "test5", UserId = userId}
            });
            var controller = new BookmarkController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.FindBookmarksAsync(userId, searchingString, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var bookmarks = Assert.IsType<List<Bookmark>>(methodResult.Value);
            Assert.Equal(3, bookmarks.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextFindBookmarksPageExistAsync_ReturnsOkWithFalse()
        {
            var userId = Guid.NewGuid();
            string searchingString = "test";
            var parameters = new BookmarkParameters { PageNumber = 2, PageSize = 3};
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextFindBookmarksPageExist(userId, searchingString, parameters)).ReturnsAsync(false);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextFindBookmarksPageExistAsync(userId, parameters, searchingString);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.False(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextFindBookmarksPageExistAsync_ReturnsOkWithTrue()
        {
            var userId = Guid.NewGuid();
            string searchingString = "test";
            var parameters = new BookmarkParameters { PageNumber = 2, PageSize = 3 };
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextFindBookmarksPageExist(userId, searchingString, parameters)).ReturnsAsync(true);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextFindBookmarksPageExistAsync(userId, parameters, searchingString);

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
