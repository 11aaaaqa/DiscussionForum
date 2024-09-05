using System.Net;
using CommentMicroservice.Api.Controllers;
using CommentMicroservice.Api.Models;
using CommentMicroservice.Api.Services.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CommentMicroservice.UnitTests
{
    public class CommentControllerTests
    {
        [Fact]
        public async Task GetAllCommentsAsync_ReturnsOkWithListOfComments()
        {
            var mock = new Mock<IRepository<Comment>>();
            mock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Comment>
            {
                new (){Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>(),
                    DiscussionId = It.IsAny<Guid>(),Id = It.IsAny<Guid>()},
                new (){Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>(),
                    DiscussionId = It.IsAny<Guid>(),Id = It.IsAny<Guid>()},
                new (){Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>(),
                    DiscussionId = It.IsAny<Guid>(),Id = It.IsAny<Guid>()}
            });
            var controller = new CommentController(mock.Object);

            var result = await controller.GetAllCommentsAsync();

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            var comments = Assert.IsType<List<Comment>>(methodResult.Value);
            Assert.Equal(3, comments.Count);
        }
    }
}
