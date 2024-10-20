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
        public async Task GetBookmarksByUserNameByNoveltyAsync_ReturnsOkWithListOfBookmarks()
        {
            var userName = "test";
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IBookmarkService>();
            mock.Setup(x => x.GetBookmarksByUserNameByNovelty(userName, bookmarkParameters)).ReturnsAsync(new List<Bookmark>
            {
                new Bookmark{Id = Guid.NewGuid(), UserName = userName}, new Bookmark{Id = Guid.NewGuid(), UserName = userName}, 
                new Bookmark{Id = Guid.NewGuid(), UserName = userName}
            });
            var controller = new BookmarkController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetBookmarksByUserNameByNoveltyAsync(userName, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var bookmarks = Assert.IsType<List<Bookmark>>(methodResult.Value);
            Assert.Equal(3, bookmarks.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetBookmarksByUserNameByAntiquityAsync_ReturnsOkWithListOfBookmarks()
        {
            var userName = "test";
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IBookmarkService>();
            mock.Setup(x => x.GetBookmarksByUserNameByAntiquity(userName, bookmarkParameters)).ReturnsAsync(new List<Bookmark>
            {
                new Bookmark{Id = Guid.NewGuid(), UserName = userName}, new Bookmark{Id = Guid.NewGuid(), UserName = userName},
                new Bookmark{Id = Guid.NewGuid(), UserName = userName}
            });
            var controller = new BookmarkController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetBookmarksByUserNameByAntiquityAsync(userName, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var bookmarks = Assert.IsType<List<Bookmark>>(methodResult.Value);
            Assert.Equal(3, bookmarks.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextBookmarksByUserNamePageExistAsync_ReturnsOkWithTrue()
        {
            var userName = "test";
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextBookmarksByUserNamePageExist(userName, bookmarkParameters)).ReturnsAsync(true);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextBookmarksByUserNamePageExistAsync(userName, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.True(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextAllBookmarksPageExistAsync_ReturnsOkWithFalse()
        {
            var userName = "test";
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextBookmarksByUserNamePageExist(userName, bookmarkParameters)).ReturnsAsync(false);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextBookmarksByUserNamePageExistAsync(userName, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.False(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task FindBookmarksByAntiquityAsync_ReturnsOkWithListOfBookmarks()
        {
            var userName = "testUSerNAme";
            string searchingString = "test";
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IBookmarkService>();
            mock.Setup(x => x.FindBookmarksByAntiquity(userName, searchingString, bookmarkParameters)).ReturnsAsync(new List<Bookmark>
            {
                new Bookmark{DiscussionTitle = "test123", UserName = userName},
                new Bookmark{DiscussionTitle = "test", UserName = userName},
                new Bookmark{DiscussionTitle = "test5", UserName = userName}
            });
            var controller = new BookmarkController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.FindBookmarksByAntiquityAsync(userName, searchingString, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var bookmarks = Assert.IsType<List<Bookmark>>(methodResult.Value);
            Assert.Equal(3, bookmarks.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task FindBookmarksByNoveltyAsync_ReturnsOkWithListOfBookmarks()
        {
            var userName = "testUSerNAme";
            string searchingString = "test";
            var bookmarkParameters = new BookmarkParameters { PageSize = 3, PageNumber = 3 };
            var mock = new Mock<IBookmarkService>();
            mock.Setup(x => x.FindBookmarksByNovelty(userName, searchingString, bookmarkParameters)).ReturnsAsync(new List<Bookmark>
            {
                new Bookmark{DiscussionTitle = "test123", UserName = userName},
                new Bookmark{DiscussionTitle = "test", UserName = userName},
                new Bookmark{DiscussionTitle = "test5", UserName = userName}
            });
            var controller = new BookmarkController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.FindBookmarksByNoveltyAsync(userName, searchingString, bookmarkParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var bookmarks = Assert.IsType<List<Bookmark>>(methodResult.Value);
            Assert.Equal(3, bookmarks.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextFindBookmarksPageExistAsync_ReturnsOkWithFalse()
        {
            var userName = "testUSerNAme";
            string searchingString = "test";
            var parameters = new BookmarkParameters { PageNumber = 2, PageSize = 3};
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextFindBookmarksPageExist(userName, searchingString, parameters)).ReturnsAsync(false);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextFindBookmarksPageExistAsync(userName, parameters, searchingString);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.False(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextFindBookmarksPageExistAsync_ReturnsOkWithTrue()
        {
            var userName = "testUSerNAme";
            string searchingString = "test";
            var parameters = new BookmarkParameters { PageNumber = 2, PageSize = 3 };
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextFindBookmarksPageExist(userName, searchingString, parameters)).ReturnsAsync(true);
            var controller = new BookmarkController(new Mock<IBookmarkService>().Object, mock.Object);

            var result = await controller.DoesNextFindBookmarksPageExistAsync(userName, parameters, searchingString);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.True(doesExist);
            mock.VerifyAll();
        }

        [Fact]
        public async Task AddBookmarkAsync_ReturnsOkWithCreatedBookmark()
        {
            var bookmarkModel = new BookmarkDto {DiscussionId = Guid.NewGuid(), DiscussionTitle = "tests", UserName = "testUsername"};
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
