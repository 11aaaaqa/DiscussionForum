using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TopicMicroservice.Api.Controllers;
using TopicMicroservice.Api.DTOs;
using TopicMicroservice.Api.Models;
using TopicMicroservice.Api.Services;
using TopicMicroservice.Api.Services.Repository;

namespace TopicMicroservice.UnitTests
{
    public class TopicControllerTests
    {
        [Fact]
        public async Task GetAllTopicsAsync_ReturnsOkWithTopics()
        {
            var topicParameters = new TopicParameters { PageNumber = It.IsAny<int>(), PageSize = It.IsAny<int>()};
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetAllAsync(topicParameters.PageSize, topicParameters.PageNumber)).ReturnsAsync(new List<Topic>
            {
                new() { Id = Guid.NewGuid(), Name = "test", PostsCount = 0 },
                new() { Id = Guid.NewGuid(), Name = "test2", PostsCount = 12 }
            });
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.GetAllTopicsAsync(topicParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var topics = Assert.IsAssignableFrom<List<Topic>>(methodResult.Value);
            Assert.Equal(2, topics.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetTopicByNameAsync_ReturnsBadRequest()
        {
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByNameAsync("incorrectName")).ReturnsAsync((Topic?)null);
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.GetTopicByNameAsync("incorrectName");

            Assert.IsType<BadRequestObjectResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetTopicByNameAsync_ReturnsOkWithTopic()
        {
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByNameAsync("correctName")).ReturnsAsync(new Topic{Id = Guid.NewGuid(), Name = "correctName", PostsCount = It.IsAny<uint>()});
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.GetTopicByNameAsync("correctName");

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var topic = Assert.IsAssignableFrom<Topic>(methodResult.Value);
            Assert.Equal("correctName", topic.Name);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetTopicByIdAsync_ReturnsBadRequest()
        {
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByIdAsync(new Guid("741ba091-7d2c-4c26-8247-f538c217c473"))).ReturnsAsync((Topic?)null);
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.GetTopicByIdAsync(new Guid("741ba091-7d2c-4c26-8247-f538c217c473"));

            Assert.IsType<BadRequestObjectResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetTopicByIdAsync_ReturnsOkWithTopic()
        {
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByIdAsync(new Guid("741ba091-7d2c-4c26-8247-f538c217c473"))).ReturnsAsync(new Topic{ Id = new Guid("741ba091-7d2c-4c26-8247-f538c217c473"), Name = It.IsAny<string>(), PostsCount = It.IsAny<uint>() });
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.GetTopicByIdAsync(new Guid("741ba091-7d2c-4c26-8247-f538c217c473"));

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var topic = Assert.IsAssignableFrom<Topic>(methodResult.Value);
            Assert.Equal(new Guid("741ba091-7d2c-4c26-8247-f538c217c473"), topic.Id);
            mock.VerifyAll();
        }

        [Fact]
        public async Task CreateTopicAsync_ReturnsBadRequest()
        {
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByNameAsync("existingName")).ReturnsAsync(new Topic
                { Id = It.IsAny<Guid>(), Name = "existingName", PostsCount = It.IsAny<uint>() });
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.CreateTopicAsync(new TopicDto { Name = "existingName" });

            Assert.IsType<BadRequestObjectResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task CreateTopicAsync_ReturnsOkWithCreatedTopic()
        {
            var model = new TopicDto { Name = "notExistingName" };
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByNameAsync(model.Name)).ReturnsAsync((Topic?)null);
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.CreateTopicAsync(model);
            
            Assert.IsType<OkObjectResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UpdateTopicAsync_ReturnsBadRequest()
        {
            var model = new Topic
            {
                Id = new Guid("20d7f6f4-3691-45fc-9057-654208ed1eae"), Name = It.IsAny<string>(),
                PostsCount = It.IsAny<uint>()
            };
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByIdAsync(model.Id)).ReturnsAsync((Topic?)null);
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.UpdateTopicAsync(model);

            Assert.IsType<BadRequestObjectResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task UpdateTopicAsync_ReturnsOkWithUpdatedTopic()
        {
            var model = new Topic
            {
                Id = new Guid("20d7f6f4-3691-45fc-9057-654208ed1eae"),
                Name = "updateTopicName",
                PostsCount = 123
            };
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByIdAsync(model.Id)).ReturnsAsync(new Topic
            {
                Id = new Guid("20d7f6f4-3691-45fc-9057-654208ed1eae"),
                Name = It.IsAny<string>(), PostsCount = It.IsAny<uint>()
            });
            mock.Setup(x => x.UpdateAsync(model)).ReturnsAsync(model);
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.UpdateTopicAsync(model);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var topic = Assert.IsType<Topic>(methodResult.Value);
            Assert.Equal(model.Id, topic.Id);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteTopicByIdAsync_ReturnsBadRequest()
        {
            var id = new Guid("e4d0f8d1-c73b-4d65-9952-72331581fe50");
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((Topic?)null);
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.DeleteTopicByIdAsync(id);

            Assert.IsType<BadRequestObjectResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteTopicByIdAsync_ReturnsOk()
        {
            var id = new Guid("e4d0f8d1-c73b-4d65-9952-72331581fe50");
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(new Topic
                { Id = id, Name = It.IsAny<string>(), PostsCount = It.IsAny<uint>() });
            mock.Setup(x => x.DeleteByIdAsync(id));
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.DeleteTopicByIdAsync(id);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteTopicByNameAsync_ReturnsBadRequest()
        {
            string name = "notExistingName";
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByNameAsync(name)).ReturnsAsync((Topic?)null);
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.DeleteTopicByNameAsync(name);

            Assert.IsType<BadRequestObjectResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteTopicByNameAsync_ReturnsOk()
        {
            string name = "existingName";
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetByNameAsync(name)).ReturnsAsync(new Topic
                { Id = It.IsAny<Guid>(), Name = name, PostsCount = It.IsAny<uint>() });
            mock.Setup(x => x.DeleteByNameAsync(name));
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object, new Mock<ITopicService>().Object);

            var result = await controller.DeleteTopicByNameAsync(name);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextTopicsPageExistAsync_ReturnsOkWithTrue()
        {
            var topicParameters = new TopicParameters { PageSize = It.IsAny<int>(), PageNumber = It.IsAny<int>() };
            var mock = new Mock<ITopicService>();
            mock.Setup(x => x.DoesAllTopicsHaveNextPage(topicParameters.PageSize, topicParameters.PageNumber)).ReturnsAsync(true);
            var controller = new TopicController(new Mock<IRepository<Topic>>().Object, new Mock<ILogger<TopicController>>().Object, mock.Object);

            var result = await controller.DoesNextTopicsPageExistAsync(topicParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var existingResult = Assert.IsType<bool>(methodResult.Value);
            Assert.True(existingResult);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DoesNextTopicsPageExistAsync_ReturnsOkWithFalse()
        {
            var topicParameters = new TopicParameters { PageSize = It.IsAny<int>(), PageNumber = It.IsAny<int>() };
            var mock = new Mock<ITopicService>();
            mock.Setup(x => x.DoesAllTopicsHaveNextPage(topicParameters.PageSize, topicParameters.PageNumber)).ReturnsAsync(false);
            var controller = new TopicController(new Mock<IRepository<Topic>>().Object, new Mock<ILogger<TopicController>>().Object, mock.Object);

            var result = await controller.DoesNextTopicsPageExistAsync(topicParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var existingResult = Assert.IsType<bool>(methodResult.Value);
            Assert.False(existingResult);
            mock.VerifyAll();
        }

        [Fact]
        public async Task FindTopicsAsync_ReturnsOkWithTopicsWhichNamesContainSearchingString()
        {
            int pageNumber = 2;
            int pageSize = 2;
            string searchingString = "test";
            var mock = new Mock<IRepository<Topic>>();
            mock.Setup(x => x.GetAllAsync(pageSize, pageNumber, searchingString)).ReturnsAsync(new List<Topic>
            {
                new (){Name = "teststetest"}, new (){Name = "123yaltestf"}
            });
            var controller = new TopicController(mock.Object, new Mock<ILogger<TopicController>>().Object,
                new Mock<ITopicService>().Object);

            var result =
                await controller.FindTopicsAsync(new TopicParameters { PageSize = pageSize, PageNumber = pageNumber },
                    searchingString);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var topics = Assert.IsType<List<Topic>>(methodResult.Value);
            Assert.Equal(2, topics.Count);
        }

        [Fact]
        public async Task DoesNextTopicsPageExistSearchingAsync_ReturnsOkWithTrue()
        {
            int pageSize = 2;
            int pageNumber = 3;
            string searchingString = "test";
            var mock = new Mock<ITopicService>();
            mock.Setup(x => x.DoesAllTopicsHaveNextPage(pageSize, pageNumber, searchingString)).ReturnsAsync(true);
            var controller = new TopicController(new Mock<IRepository<Topic>>().Object, new Mock<ILogger<TopicController>>().Object,
                mock.Object);

            var result = await controller.DoesNextTopicsPageExistSearchingAsync(new TopicParameters
            {
                PageSize = pageSize, PageNumber = pageNumber
            }, searchingString);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.True(doesExist);
        }

        [Fact]
        public async Task DoesNextTopicsPageExistSearchingAsync_ReturnsOkWithFalse()
        {
            int pageSize = 2;
            int pageNumber = 3;
            string searchingString = "test";
            var mock = new Mock<ITopicService>();
            mock.Setup(x => x.DoesAllTopicsHaveNextPage(pageSize, pageNumber, searchingString)).ReturnsAsync(false);
            var controller = new TopicController(new Mock<IRepository<Topic>>().Object, new Mock<ILogger<TopicController>>().Object,
                mock.Object);

            var result = await controller.DoesNextTopicsPageExistSearchingAsync(new TopicParameters
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            }, searchingString);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.False(doesExist);
        }
    }
}
