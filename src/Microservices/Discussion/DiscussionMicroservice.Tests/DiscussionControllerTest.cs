using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using DiscussionMicroservice.Api.Controllers;
using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DiscussionMicroservice.Tests
{
    public class DiscussionControllerTest
    {
        [Fact]
        public async Task GetDiscussionsByTopicNameAsync_ReturnsOkWithListOfTopics()
        {
            var mock = new Mock<ApplicationDbContext>();
            var discussions = new List<Discussion>
            {
                new Discussion
                {
                    Content = It.IsAny<string>(), CreatedAt = It.IsAny<DateOnly>(), Id = It.IsAny<Guid>(), CreatedBy = It.IsAny<string>(),
                    Rating = It.IsAny<int>(), Title = It.IsAny<string>(), TopicName = "TestTopicName"
                },new Discussion
                {
                    Content = It.IsAny<string>(), CreatedAt = It.IsAny<DateOnly>(), Id = It.IsAny<Guid>(), CreatedBy = It.IsAny<string>(),
                    Rating = It.IsAny<int>(), Title = It.IsAny<string>(), TopicName = "TestTopicName"
                },new Discussion
                {
                    Content = It.IsAny<string>(), CreatedAt = It.IsAny<DateOnly>(), Id = It.IsAny<Guid>(), CreatedBy = It.IsAny<string>(),
                    Rating = It.IsAny<int>(), Title = It.IsAny<string>(), TopicName = "TestTopicName"
                }
            };

            mock.Setup(x => x.Discussions.Where(x => x.TopicName == "TestTopicName").ToList()).Returns(discussions);

            var controller = new DiscussionController(mock.Object);

            var result = await controller.GetDiscussionsByTopicNameAsync("TestTopicName");

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
