using BanHistoryMicroservice.Api.Controllers;
using BanHistoryMicroservice.Api.Models;
using BanHistoryMicroservice.Api.Services;
using BanHistoryMicroservice.Api.Services.Pagination;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BanHistoryMicroservice.UnitTests
{
    public class BanHistoryControllerTests
    {
        [Fact]
        public async Task GetAllBansAsync_ReturnsOkWithEmptyListOfBans()
        {
            var banHistoryParameters = new BanHistoryParameters { PageSize = 5, PageNumber = 10};
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.GetAllBansAsync(banHistoryParameters)).ReturnsAsync(new List<Ban>());
            var controller = new BanHistoryController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetAllBansAsync(banHistoryParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult);
            Assert.Equal(200, methodResult.StatusCode);
            var bans = Assert.IsType<List<Ban>>(methodResult.Value);
            Assert.Empty(bans);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetAllBansAsync_ReturnsOkWithListOfBans()
        {
            var banHistoryParameters = new BanHistoryParameters { PageSize = 3, PageNumber = 2 };
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.GetAllBansAsync(banHistoryParameters)).ReturnsAsync(new List<Ban>
            {
                new ()
                {
                    BanType = It.IsAny<string>(), DurationInDays = It.IsAny<uint>(), Id = It.IsAny<Guid>(), UserName = It.IsAny<string>(),
                    Reason = It.IsAny<string>(), UserId = It.IsAny<Guid>()
                },
                new ()
                {
                    BanType = It.IsAny<string>(), DurationInDays = It.IsAny<uint>(), Id = It.IsAny<Guid>(), UserName = It.IsAny<string>(),
                    Reason = It.IsAny<string>(), UserId = It.IsAny<Guid>()
                },
                new ()
                {
                    BanType = It.IsAny<string>(), DurationInDays = It.IsAny<uint>(), Id = It.IsAny<Guid>(), UserName = It.IsAny<string>(),
                    Reason = It.IsAny<string>(), UserId = It.IsAny<Guid>()
                }
            });
            var controller = new BanHistoryController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetAllBansAsync(banHistoryParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult);
            Assert.Equal(200, methodResult.StatusCode);
            var bans = Assert.IsType<List<Ban>>(methodResult.Value);
            Assert.Equal(3, bans.Count);
            mock.Verify(x => x.GetAllBansAsync(banHistoryParameters));
        }

        [Fact]
        public async Task GetBansByUserIdAsync_ReturnsOkWithListOfBansWithNeededUserId()
        {
            var banHistoryParameters = new BanHistoryParameters { PageSize = 3, PageNumber = 2 };
            var id = It.IsAny<Guid>();
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.GetByUserIdAsync(id, banHistoryParameters)).ReturnsAsync(new List<Ban>
            {
                new ()
                {
                    BanType = It.IsAny<string>(), DurationInDays = It.IsAny<uint>(), Id = It.IsAny<Guid>(), UserName = It.IsAny<string>(),
                    Reason = It.IsAny<string>(), UserId = id
                },
                new ()
                {
                    BanType = It.IsAny<string>(), DurationInDays = It.IsAny<uint>(), Id = It.IsAny<Guid>(), UserName = It.IsAny<string>(),
                    Reason = It.IsAny<string>(), UserId = id
                }
            });
            var controller = new BanHistoryController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetBansByUserIdAsync(id, banHistoryParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            Assert.Equal(200, methodResult.StatusCode);
            var bans = Assert.IsType<List<Ban>>(methodResult.Value);
            Assert.Equal(2, bans.Count);
            foreach (var ban in bans)
            {
                Assert.Equal(id, ban.UserId);
            }
            mock.Verify(x => x.GetByUserIdAsync(id, banHistoryParameters));
        }

        [Fact]
        public async Task GetBansByUserNameAsync_ReturnsOkWithListOfBansWithNeededUserName()
        {
            var banHistoryParameters = new BanHistoryParameters { PageSize = 3, PageNumber = 2 };
            var userName = It.IsAny<string>();
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.GetByUserNameAsync(userName, banHistoryParameters)).ReturnsAsync(new List<Ban>
            {
                new ()
                {
                    BanType = It.IsAny<string>(), DurationInDays = It.IsAny<uint>(), Id = It.IsAny<Guid>(), UserName = userName,
                    Reason = It.IsAny<string>(), UserId = It.IsAny<Guid>()
                },
                new ()
                {
                    BanType = It.IsAny<string>(), DurationInDays = It.IsAny<uint>(), Id = It.IsAny<Guid>(), UserName = userName,
                    Reason = It.IsAny<string>(), UserId = It.IsAny<Guid>()
                }
            });
            var controller = new BanHistoryController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetBansByUserNameAsync(userName , banHistoryParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            Assert.Equal(200, methodResult.StatusCode);
            var bans = Assert.IsType<List<Ban>>(methodResult.Value);
            Assert.Equal(2, bans.Count);
            foreach (var ban in bans)
            {
                Assert.Equal(userName, ban.UserName);
            }
            mock.Verify(x => x.GetByUserNameAsync(userName, banHistoryParameters));
        }

        [Fact]
        public async Task GetBansByBanTypeAsync_ReturnsOkWithListOfBansWithNeededBanType()
        {
            var banHistoryParameters = new BanHistoryParameters { PageSize = 3, PageNumber = 2 };
            var banType = It.IsAny<string>();
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.GetByBanTypeAsync(banType , banHistoryParameters)).ReturnsAsync(new List<Ban>
            {
                new ()
                {
                    BanType = banType, DurationInDays = It.IsAny<uint>(), Id = It.IsAny<Guid>(), UserName = It.IsAny<string>(),
                    Reason = It.IsAny<string>(), UserId = It.IsAny<Guid>()
                },
                new ()
                {
                    BanType = banType, DurationInDays = It.IsAny<uint>(), Id = It.IsAny<Guid>(), UserName = It.IsAny<string>(),
                    Reason = It.IsAny<string>(), UserId = It.IsAny<Guid>()
                },
                new ()
                {
                    BanType = banType, DurationInDays = It.IsAny<uint>(), Id = It.IsAny<Guid>(), UserName = It.IsAny<string>(),
                    Reason = It.IsAny<string>(), UserId = It.IsAny<Guid>()
                }
            });
            var controller = new BanHistoryController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetBansByBanTypeAsync(banType , banHistoryParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            Assert.Equal(200, methodResult.StatusCode);
            var bans = Assert.IsType<List<Ban>>(methodResult.Value);
            Assert.Equal(3, bans.Count);
            foreach (var ban in bans)
            {
                Assert.Equal(banType, ban.BanType);
            }
            mock.Verify(x => x.GetByBanTypeAsync(banType, banHistoryParameters));
        }

        [Fact]
        public async Task GetBanByIdAsync_ReturnsBadRequest()
        {
            var id = It.IsAny<Guid>();
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((Ban?)null);
            var controller = new BanHistoryController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetBanByIdAsync(id);

            Assert.IsType<BadRequestResult>(result);
            mock.Verify(x => x.GetByIdAsync(id));
        }

        [Fact]
        public async Task GetBanByIdAsync_ReturnsOkWithBan()
        {
            var id = It.IsAny<Guid>();
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(new Ban
            {
                BanType = It.IsAny<string>(), DurationInDays = It.IsAny<uint>(), 
                Id = id, UserName = It.IsAny<string>(), Reason = It.IsAny<string>(), UserId = It.IsAny<Guid>()
            });
            var controller = new BanHistoryController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetBanByIdAsync(id);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var ban = Assert.IsType<Ban>(methodResult.Value);
            Assert.Equal(id, ban.Id);
            mock.Verify(x => x.GetByIdAsync(id));
        }

        [Fact]
        public async Task DeleteBanByIdAsync_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.DeleteAsync(id));
            var controller = new BanHistoryController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.DeleteBanByIdAsync(id);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteBansByUserIdAsync_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.DeleteBansByUserId(userId));
            var controller = new BanHistoryController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.DeleteBansByUserIdAsync(userId);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteBansByUserNameAsync_ReturnsOk()
        {
            var userName = It.IsAny<string>();
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.DeleteBansByUserName(userName));
            var controller = new BanHistoryController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.DeleteBansByUserNameAsync(userName);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }
    }
}
