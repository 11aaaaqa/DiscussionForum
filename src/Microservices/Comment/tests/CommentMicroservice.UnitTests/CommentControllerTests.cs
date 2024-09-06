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

        [Fact]
        public async Task GetCommentsByDiscussionIdAsync_ReturnsBadRequest()
        {
            var id = It.IsAny<Guid>();
            var mock = new Mock<IRepository<Comment>>();
            mock.Setup(x => x.GetByDiscussionIdAsync(id)).ReturnsAsync((List<Comment>?)null);
            var controller = new CommentController(mock.Object);

            var result = await controller.GetCommentsByDiscussionIdAsync(id);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetCommentsByDiscussionIdAsync_ReturnsOkWithListOfComments()
        {
            var discussionId = It.IsAny<Guid>();
            var mock = new Mock<IRepository<Comment>>();
            mock.Setup(x => x.GetByDiscussionIdAsync(discussionId)).ReturnsAsync(new List<Comment>
            {
                new(){DiscussionId = discussionId, Id=It.IsAny<Guid>(), CreatedBy = It.IsAny<string>(),
                    Content = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>()},
                new(){DiscussionId = discussionId, Id=It.IsAny<Guid>(), CreatedBy = It.IsAny<string>(),
                    Content = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>()}
            });
            var controller = new CommentController(mock.Object);

            var result = await controller.GetCommentsByDiscussionIdAsync(discussionId);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            var comments = Assert.IsType<List<Comment>>(methodResult.Value);
            Assert.Equal(2, comments.Count);
        }

        [Fact]
        public async Task UpdateCommentAsync_ReturnsBadRequest()
        {
            Comment model = new()
            {
                Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>(),
                DiscussionId = It.IsAny<Guid>(), Id = It.IsAny<Guid>()
            };
            var mock = new Mock<IRepository<Comment>>();
            mock.Setup(x => x.GetByIdAsync(model.Id)).ReturnsAsync((Comment?)null);
            var controller = new CommentController(mock.Object);

            var result = await controller.UpdateCommentAsync(model);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateCommentAsync_ReturnsOkWithUpdatedComment()
        {
            Comment model = new()
            {
                Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>(), 
                DiscussionId = It.IsAny<Guid>(), Id = It.IsAny<Guid>()
            };
            var mock = new Mock<IRepository<Comment>>();
            mock.Setup(x => x.GetByIdAsync(model.Id)).ReturnsAsync(new Comment
            {
                Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>(),
                DiscussionId = It.IsAny<Guid>(), Id = model.Id
            });
            mock.Setup(x => x.UpdateAsync(model)).ReturnsAsync(model);
            var controller = new CommentController(mock.Object);

            var result = await controller.UpdateCommentAsync(model);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            var updatedComment = Assert.IsType<Comment>(methodResult.Value);
            Assert.Equal(model.Id, updatedComment.Id);
            Assert.Equal(model.DiscussionId, updatedComment.DiscussionId);
            Assert.Equal(model.Content, updatedComment.Content);
            Assert.Equal(model.CreatedBy, updatedComment.CreatedBy);
            Assert.Equal(model.CreatedDate, updatedComment.CreatedDate);
        }
    }
}
