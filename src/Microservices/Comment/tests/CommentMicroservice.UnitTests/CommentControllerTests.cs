using CommentMicroservice.Api.Controllers;
using CommentMicroservice.Api.Models;
using CommentMicroservice.Api.Services;
using CommentMicroservice.Api.Services.Repository;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CommentMicroservice.UnitTests
{
    public class CommentControllerTests
    {
        [Fact]
        public async Task GetAllCommentsAsync_ReturnsOkWithListOfComments()
        {
            var commentParameters = new CommentParameters { PageSize = 3, PageNumber = 2 };
            var mock = new Mock<IRepository<Comment>>();
            mock.Setup(x => x.GetAllAsync(commentParameters)).ReturnsAsync(new List<Comment>
            {
                new (){Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>(),
                    DiscussionId = It.IsAny<Guid>(),Id = It.IsAny<Guid>()},
                new (){Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>(),
                    DiscussionId = It.IsAny<Guid>(),Id = It.IsAny<Guid>()},
                new (){Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>(),
                    DiscussionId = It.IsAny<Guid>(),Id = It.IsAny<Guid>()}
            });
            var controller = new CommentController(mock.Object, new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetAllCommentsAsync(commentParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            var comments = Assert.IsType<List<Comment>>(methodResult.Value);
            Assert.Equal(3, comments.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetCommentsByDiscussionIdAsync_ReturnsOkWithListOfComments()
        {
            var commentParameters = new CommentParameters { PageSize = 2, PageNumber = 5};
            var discussionId = It.IsAny<Guid>();
            var mock = new Mock<IRepository<Comment>>();
            mock.Setup(x => x.GetByDiscussionIdAsync(discussionId, commentParameters)).ReturnsAsync(new List<Comment>
            {
                new(){DiscussionId = discussionId, Id=It.IsAny<Guid>(), CreatedBy = It.IsAny<string>(),
                    Content = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>()},
                new(){DiscussionId = discussionId, Id=It.IsAny<Guid>(), CreatedBy = It.IsAny<string>(),
                    Content = It.IsAny<string>(), CreatedDate = It.IsAny<DateTime>()}
            });
            var controller = new CommentController(mock.Object, new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetCommentsByDiscussionIdAsync(discussionId, commentParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            var comments = Assert.IsType<List<Comment>>(methodResult.Value);
            Assert.Equal(2, comments.Count);
            mock.VerifyAll();
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
            var controller = new CommentController(mock.Object, new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.UpdateCommentAsync(model);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
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
            var controller = new CommentController(mock.Object, new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.UpdateCommentAsync(model);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            var updatedComment = Assert.IsType<Comment>(methodResult.Value);
            Assert.Equal(model.Id, updatedComment.Id);
            Assert.Equal(model.DiscussionId, updatedComment.DiscussionId);
            Assert.Equal(model.Content, updatedComment.Content);
            Assert.Equal(model.CreatedBy, updatedComment.CreatedBy);
            Assert.Equal(model.CreatedDate, updatedComment.CreatedDate);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteCommentByIdAsync_BadRequest()
        {
            var id = It.IsAny<Guid>();
            var mock = new Mock<IRepository<Comment>>();
            mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((Comment?)null);
            var controller = new CommentController(mock.Object, new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.DeleteCommentByIdAsync(id);

            Assert.IsType<BadRequestResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteCommentByIdAsync_ReturnsOk()
        {
            var id = It.IsAny<Guid>();
            var mock = new Mock<IRepository<Comment>>();
            mock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(new Comment { Id = id, CreatedBy = It.IsAny<string>()});
            mock.Setup(x => x.DeleteByIdAsync(id));
            var controller = new CommentController(mock.Object, new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.DeleteCommentByIdAsync(id);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetCommentsByIdsAsync_ReturnsOkWithListOfComments()
        {
            var id1 = Guid.NewGuid(); var id2 = Guid.NewGuid(); var id3 = Guid.NewGuid();
            var mock = new Mock<IRepository<Comment>>();
            mock.Setup(x => x.GetByIds(id1, id2, id3)).ReturnsAsync(new List<Comment>
            {
                new Comment{Id = id1}, new Comment{Id = id2}, new Comment{Id = id3}
            });
            var controller = new CommentController(mock.Object, new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetCommentsByIdsAsync(id1, id2, id3);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var comments = Assert.IsType<List<Comment>>(methodResult.Value);
            Assert.Equal(3, comments.Count);
            mock.Verify(x => x.GetByIds(id1,id2,id3));
        }
    }
}
