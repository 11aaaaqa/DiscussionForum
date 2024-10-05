using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscussionMicroservice.Api.Controllers;
using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.Models;
using DiscussionMicroservice.Api.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DiscussionMicroservice.UnitTests
{
    public class DiscussionControllerTests
    {
        [Fact]
        public async Task GetAllDiscussionsSortedByNoveltyAsync_ReturnsOkWithListOfDiscussions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var discussionParameters = new DiscussionParameters { PageSize = 5, PageNumber = 2 };
            string topicName = It.IsAny<string>();
            var mock = new Mock<IGetAllDiscussionsService>();
            mock.Setup(x => x.GetAllDiscussionsSortedByNovelty(discussionParameters, topicName)).ReturnsAsync(
                new List<Discussion>
                {
                    new (){Id = It.IsAny<Guid>()}, new (){Id = It.IsAny<Guid>()}, new (){Id = It.IsAny<Guid>()}
                });
            var controller = new DiscussionController(new Mock<ApplicationDbContext>(optionsBuilder.Options).Object,
                new Mock<IPublishEndpoint>().Object,
                mock.Object);

            var result = await controller.GetAllDiscussionsSortedByNoveltyAsync(discussionParameters, topicName);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var discussions = Assert.IsType<List<Discussion>>(methodResult.Value);
            Assert.Equal(3, discussions.Count);
        }

        [Fact]
        public async Task GetAllDiscussionsSortedByPopularityAsync_ReturnsOkWithListOfDiscussions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var discussionParameters = new DiscussionParameters { PageSize = 5, PageNumber = 2 };
            string topicName = It.IsAny<string>();
            var mock = new Mock<IGetAllDiscussionsService>();
            mock.Setup(x => x.GetAllDiscussionsSortedByPopularity(discussionParameters, topicName)).ReturnsAsync(
                new List<Discussion>
                {
                    new (){Id = It.IsAny<Guid>()}, new (){Id = It.IsAny<Guid>()}, new (){Id = It.IsAny<Guid>()}
                });
            var controller = new DiscussionController(new Mock<ApplicationDbContext>(optionsBuilder.Options).Object,
                new Mock<IPublishEndpoint>().Object,
                mock.Object);

            var result = await controller.GetAllDiscussionsSortedByPopularityAsync(discussionParameters, topicName);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var discussions = Assert.IsType<List<Discussion>>(methodResult.Value);
            Assert.Equal(3, discussions.Count);
        }
    }
}
