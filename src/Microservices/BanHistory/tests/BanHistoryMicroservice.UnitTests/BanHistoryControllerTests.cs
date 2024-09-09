using BanHistoryMicroservice.Api.Controllers;
using BanHistoryMicroservice.Api.Models;
using BanHistoryMicroservice.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BanHistoryMicroservice.UnitTests
{
    public class BanHistoryControllerTests
    {
        [Fact]
        public async Task GetAllBansAsync_ReturnsOkWithEmptyListOfBans()
        {
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.GetAllBansAsync()).ReturnsAsync(new List<Ban>());
            var controller = new BanHistoryController(mock.Object);

            var result = await controller.GetAllBansAsync();

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult);
            Assert.Equal(200, methodResult.StatusCode);
            var bans = Assert.IsType<List<Ban>>(methodResult.Value);
            Assert.Empty(bans);
            mock.Verify(x => x.GetAllBansAsync());
        }

        [Fact]
        public async Task GetAllBansAsync_ReturnsOkWithListOfBans()
        {
            var mock = new Mock<IBanService<Ban>>();
            mock.Setup(x => x.GetAllBansAsync()).ReturnsAsync(new List<Ban>
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
            var controller = new BanHistoryController(mock.Object);

            var result = await controller.GetAllBansAsync();

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult);
            Assert.Equal(200, methodResult.StatusCode);
            var bans = Assert.IsType<List<Ban>>(methodResult.Value);
            Assert.Equal(3, bans.Count);
            mock.Verify(x => x.GetAllBansAsync());
        }
    }
}
