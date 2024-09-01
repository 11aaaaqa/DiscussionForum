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
    }
}
