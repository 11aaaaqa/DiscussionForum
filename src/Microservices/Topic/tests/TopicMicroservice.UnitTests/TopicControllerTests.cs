using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TopicMicroservice.Api.Controllers;
using TopicMicroservice.Api.Models;
using TopicMicroservice.Api.Services.Repository;

namespace TopicMicroservice.UnitTests
{
    public class TopicControllerTests
    {
        [Fact]
        public async Task GetAllTopicsAsync_ReturnsOkWithTopics()
        {
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Topic>
            {
                new() { Id = Guid.NewGuid(), Name = "test", PostsCount = 0 },
                new() { Id = Guid.NewGuid(), Name = "test2", PostsCount = 12 }
            });
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object);

            var result = await controller.GetAllTopicsAsync();

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var topics = Assert.IsAssignableFrom<List<Topic>>(methodResult.Value);
            Assert.Equal(2, topics.Count);
        }

        [Fact]
        public async Task GetTopicByNameAsync_ReturnsBadRequest()
        {
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByNameAsync("incorrectName")).ReturnsAsync((Topic?)null);
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object);

            var result = await controller.GetTopicByNameAsync("incorrectName");

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetTopicByNameAsync_ReturnsOkWithTopic()
        {
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByNameAsync("correctName")).ReturnsAsync(new Topic{Id = Guid.NewGuid(), Name = "correctName", PostsCount = It.IsAny<uint>()});
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object);

            var result = await controller.GetTopicByNameAsync("correctName");

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var topic = Assert.IsAssignableFrom<Topic>(methodResult.Value);
            Assert.Equal("correctName", topic.Name);
        }
    }
}
