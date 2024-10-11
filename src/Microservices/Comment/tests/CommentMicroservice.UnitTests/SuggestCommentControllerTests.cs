using CommentMicroservice.Api.Controllers;
using CommentMicroservice.Api.Models;
using CommentMicroservice.Api.Services;
using CommentMicroservice.Api.Services.Repository;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using IPublishEndpoint = MassTransit.IPublishEndpoint;

namespace CommentMicroservice.UnitTests
{
    public class SuggestCommentControllerTests
    {
        [Fact]
        public async Task GetAllSuggestedCommentsAsync_ReturnsOkWithSuggestedComments()
        {
            var commentParameters = new CommentParameters { PageSize = 3, PageNumber = 2};
            var mock = new Mock<IRepository<SuggestedComment>>();
            mock.Setup(x => x.GetAllAsync(commentParameters)).ReturnsAsync(new List<SuggestedComment>
            {
                new() {Id = It.IsAny<Guid>(), Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(),
                    CreatedDate = It.IsAny<DateTime>(), DiscussionId = It.IsAny<Guid>()},
                new() {Id = It.IsAny<Guid>(), Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(),
                    CreatedDate = It.IsAny<DateTime>(), DiscussionId = It.IsAny<Guid>()},
                new() {Id = It.IsAny<Guid>(), Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(),
                    CreatedDate = It.IsAny<DateTime>(), DiscussionId = It.IsAny<Guid>()}
            });
            var controller = new SuggestCommentController(mock.Object, new Mock<IRepository<Comment>>().Object,
                new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetAllSuggestedCommentsAsync(commentParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            var comments = Assert.IsType<List<SuggestedComment>>(methodResult.Value);
            Assert.Equal(3, comments.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetSuggestedCommentsByIdsAsync_ReturnsOkWithListOfSuggestedComments()
        {
            var ids = new Guid[]
            {
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() 
            };
            var mock = new Mock<IRepository<SuggestedComment>>();
            mock.Setup(x => x.GetByIds(ids)).ReturnsAsync(new List<SuggestedComment>
            {
                new(){Id = ids[0]}, new (){Id = ids[1]}, new(){Id = ids[2]}
            });
            var controller = new SuggestCommentController(mock.Object, new Mock<IRepository<Comment>>().Object,
                new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetSuggestedCommentsByIdsAsync(ids);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var suggestedComments = Assert.IsType<List<SuggestedComment>>(methodResult.Value);
            Assert.Equal(3, suggestedComments.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetSuggestedCommentsByUserNameAsync_ReturnsOkWithListOfSuggestedCommentsWithSpecifiedUserName()
        {
            var commentParameters = new CommentParameters { PageSize = 2, PageNumber = 5 };
            string userName = It.IsAny<string>();
            var mock = new Mock<IRepository<SuggestedComment>>();
            mock.Setup(x => x.GetByUserNameAsync(userName, commentParameters)).ReturnsAsync(new List<SuggestedComment>
            {
                new (){CreatedBy = userName}, new (){ CreatedBy = userName}
            });
            var controller = new SuggestCommentController(mock.Object, new Mock<IRepository<Comment>>().Object,
                new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetSuggestedCommentsByUserNameAsync(userName, commentParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var suggestedComments = Assert.IsType<List<SuggestedComment>>(methodResult.Value);
            Assert.Equal(2, suggestedComments.Count);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteAllSuggestedCommentsByUserNameAsync_ReturnsOk()
        {
            string userName = It.IsAny<string>();
            var mock = new Mock<IRepository<SuggestedComment>>();
            mock.Setup(x => x.DeleteByUserNameAsync(userName));
            var controller = new SuggestCommentController(mock.Object, new Mock<IRepository<Comment>>().Object,
                new Mock<IPublishEndpoint>().Object, new Mock<IPaginationService>().Object);

            var result = await controller.DeleteAllSuggestedCommentsByUserNameAsync(userName);

            Assert.IsType<OkResult>(result);
            mock.VerifyAll();
        }
    }
}
