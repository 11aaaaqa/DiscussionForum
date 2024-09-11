using Microsoft.AspNetCore.Mvc;
using Moq;
using UserMicroservice.Api.Controllers;
using UserMicroservice.Api.DTOs;
using UserMicroservice.Api.Models;
using UserMicroservice.Api.Services.Ban;
using UserMicroservice.Api.Services.User;

namespace UserMicroservice.UnitTests
{
    public class UserControllerTests
    {
        [Fact]
        public async Task GetUserByUserNameAsync_ReturnsOkWithUser()
        {
            string userName = It.IsAny<string>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetUserByUserName(userName)).ReturnsAsync(new User
            {
                Id = It.IsAny<Guid>(), Answers = It.IsAny<uint>(), UserName = userName, RegisteredAt = It.IsAny<DateOnly>(),
                Posts = It.IsAny<uint>(),BanType = It.IsAny<string>(), SuggestedDiscussionsIds = It.IsAny<List<Guid>>(),
                SuggestedCommentsIds = It.IsAny<List<Guid>>(), IsBanned = It.IsAny<bool>(), CreatedDiscussionsIds = It.IsAny<List<Guid>>(),
                CommentsIds = It.IsAny<List<Guid>>(), BannedFor = It.IsAny<string>(), BannedUntil = It.IsAny<DateTime>(), AspNetUserId = It.IsAny<string>()
            });
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetUserByUserNameAsync(userName);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var user = Assert.IsType<User>(methodResult.Value);
            Assert.Equal(userName, user.UserName);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetUserByUserNameAsync_ReturnsBadRequest()
        {
            string userName = It.IsAny<string>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetUserByUserName(userName)).ReturnsAsync((User?)null);
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetUserByUserNameAsync(userName);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsOkWithUser()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(new User
            {
                Id = userId,
                Answers = It.IsAny<uint>(),
                UserName = It.IsAny<string>(),
                RegisteredAt = It.IsAny<DateOnly>(),
                Posts = It.IsAny<uint>(),
                BanType = It.IsAny<string>(),
                SuggestedDiscussionsIds = It.IsAny<List<Guid>>(),
                SuggestedCommentsIds = It.IsAny<List<Guid>>(),
                IsBanned = It.IsAny<bool>(),
                CreatedDiscussionsIds = It.IsAny<List<Guid>>(),
                CommentsIds = It.IsAny<List<Guid>>(),
                BannedFor = It.IsAny<string>(),
                BannedUntil = It.IsAny<DateTime>(),
                AspNetUserId = It.IsAny<string>()
            });
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetUserByIdAsync(userId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var user = Assert.IsType<User>(methodResult.Value);
            Assert.Equal(userId, user.Id);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsBadRequest()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetUserByIdAsync(userId);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetCreatedDiscussionsIdsByUserIdAsync_ReturnsOkWithListOfDiscussionIds()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetCreatedDiscussionsIdsByUserIdAsync(userId)).ReturnsAsync(new List<Guid>
            {
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()
            });
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetCreatedDiscussionsIdsByUserIdAsync(userId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var ids = Assert.IsType<List<Guid>>(methodResult.Value);
            Assert.Equal(3, ids.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetCreatedDiscussionsIdsByUserIdAsync_ReturnsBadRequest()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetCreatedDiscussionsIdsByUserIdAsync(userId)).ReturnsAsync((List<Guid>?)null);
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetCreatedDiscussionsIdsByUserIdAsync(userId);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetSuggestedDiscussionsIdsByUserIdAsync_ReturnsOkWithListOfSuggestedDiscussionIds()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetSuggestedDiscussionsIdsByUserIdAsync(userId)).ReturnsAsync(new List<Guid>
            {
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()
            });
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetSuggestedDiscussionsIdsByUserIdAsync(userId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var ids = Assert.IsType<List<Guid>>(methodResult.Value);
            Assert.Equal(3, ids.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetSuggestedDiscussionsIdsByUserIdAsync_ReturnsBadRequest()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetSuggestedDiscussionsIdsByUserIdAsync(userId)).ReturnsAsync((List<Guid>?)null);
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetSuggestedDiscussionsIdsByUserIdAsync(userId);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetSuggestedCommentsIdsByUserIdAsync_ReturnsOkWithListOfSuggestedCommentsIds()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetSuggestedCommentsIdsByUserIdAsync(userId)).ReturnsAsync(new List<Guid>
            {
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()
            });
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetSuggestedCommentsIdsByUserIdAsync(userId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var ids = Assert.IsType<List<Guid>>(methodResult.Value);
            Assert.Equal(3, ids.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetSuggestedCommentsIdsByUserIdAsync_ReturnsBadRequest()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetSuggestedCommentsIdsByUserIdAsync(userId)).ReturnsAsync((List<Guid>?)null);
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetSuggestedCommentsIdsByUserIdAsync(userId);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetCommentsIdsByUserIdAsync_ReturnsOkWithListOfCommentsIds()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetCommentsIdsByUserIdAsync(userId)).ReturnsAsync(new List<Guid>
            {
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()
            });
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetCommentsIdsByUserIdAsync(userId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var ids = Assert.IsType<List<Guid>>(methodResult.Value);
            Assert.Equal(3, ids.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetCommentsIdsByUserIdAsync_ReturnsBadRequest()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IUserService<User>>();
            mock.Setup(x => x.GetCommentsIdsByUserIdAsync(userId)).ReturnsAsync((List<Guid>?)null);
            var controller = new UserController(mock.Object, new Mock<IBanService<User>>().Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.GetCommentsIdsByUserIdAsync(userId);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task IsUserBannedAsync_ReturnsOkWithFalse()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IBanService<User>>();
            mock.Setup(x => x.IsUserBannedAsync(userId)).ReturnsAsync(false);
            var controller = new UserController(new Mock<IUserService<User>>().Object, mock.Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.IsUserBannedAsync(userId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var isBanned = Assert.IsType<bool>(methodResult.Value);
            Assert.False(isBanned);
            mock.VerifyAll();
        }

        [Fact]
        public async Task IsUserBannedAsync_ReturnsOkWithTrue()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IBanService<User>>();
            mock.Setup(x => x.IsUserBannedAsync(userId)).ReturnsAsync(true);
            var controller = new UserController(new Mock<IUserService<User>>().Object, mock.Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.IsUserBannedAsync(userId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var isBanned = Assert.IsType<bool>(methodResult.Value);
            Assert.True(isBanned);
            mock.VerifyAll();
        }

        [Fact]
        public async Task BanUserByIdAsync_ReturnsOk()
        {
            var userId = It.IsAny<Guid>();
            var reason = It.IsAny<string>();
            var banType = It.IsAny<string>();
            var forDays = It.IsAny<uint>();
            var mock = new Mock<IBanService<User>>();
            mock.Setup(x => x.BanUserAsync(userId, reason, banType, forDays));
            var controller = new UserController(new Mock<IUserService<User>>().Object, mock.Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.BanUserByIdAsync(userId,
                new BanDto { BanType = banType, ForDays = forDays, Reason = reason });

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task BanUserByUserNameAsync_ReturnsOk()
        {
            var userName = It.IsAny<string>();
            var reason = It.IsAny<string>();
            var banType = It.IsAny<string>();
            var forDays = It.IsAny<uint>();
            var mock = new Mock<IBanService<User>>();
            mock.Setup(x => x.BanUserAsync(userName, reason, banType, forDays));
            var controller = new UserController(new Mock<IUserService<User>>().Object, mock.Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.BanUserByUserNameAsync(userName,
                new BanDto { BanType = banType, ForDays = forDays, Reason = reason });

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UnbanUserByUserIdAsync_ReturnsOk()
        {
            var userId = It.IsAny<Guid>();
            var mock = new Mock<IBanService<User>>();
            mock.Setup(x => x.UnbanUserAsync(userId));
            var controller = new UserController(new Mock<IUserService<User>>().Object, mock.Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.UnbanUserByUserIdAsync(userId);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UnbanUserByUserNameAsync_ReturnsOk()
        {
            var userName = It.IsAny<string>();
            var mock = new Mock<IBanService<User>>();
            mock.Setup(x => x.UnbanUserAsync(userName));
            var controller = new UserController(new Mock<IUserService<User>>().Object, mock.Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.UnbanUserByUserNameAsync(userName);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task ChangeUserNameAsync_ReturnsBadRequest()
        {
            var userId = It.IsAny<Guid>();
            var newUserName = It.IsAny<string>();
            var mock = new Mock<IChangeUserName>();
            mock.Setup(x => x.ChangeUserNameAsync(userId, newUserName)).ReturnsAsync(false);
            var controller = new UserController(new Mock<IUserService<User>>().Object,
                new Mock<IBanService<User>>().Object, mock.Object);

            var result = await controller.ChangeUserNameAsync(userId, newUserName);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task ChangeUserNameAsync_ReturnsOk()
        {
            var userId = It.IsAny<Guid>();
            var newUserName = It.IsAny<string>();
            var mock = new Mock<IChangeUserName>();
            mock.Setup(x => x.ChangeUserNameAsync(userId, newUserName)).ReturnsAsync(true);
            var controller = new UserController(new Mock<IUserService<User>>().Object,
                new Mock<IBanService<User>>().Object, mock.Object);

            var result = await controller.ChangeUserNameAsync(userId, newUserName);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task IsUserBannedUserNameOverload_ReturnsOk(bool isBanned)
        {
            string userName = It.IsAny<string>();
            var mock = new Mock<IBanService<User>>();
            mock.Setup(x => x.IsUserBannedAsync(userName)).ReturnsAsync(isBanned);
            var controller = new UserController(new Mock<IUserService<User>>().Object, mock.Object,
                new Mock<IChangeUserName>().Object);

            var result = await controller.IsUserBannedAsync(userName);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            Assert.Equal(200, methodResult.StatusCode);
            var isUserBannedResult = Assert.IsType<bool>(methodResult.Value);
            Assert.Equal(isBanned, isUserBannedResult);
            mock.VerifyAll();
        }
    }
}
