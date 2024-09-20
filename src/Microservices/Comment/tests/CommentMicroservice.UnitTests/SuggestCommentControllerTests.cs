using CommentMicroservice.Api.Controllers;
using CommentMicroservice.Api.Models;
using CommentMicroservice.Api.Services.Repository;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CommentMicroservice.UnitTests
{
    public class SuggestCommentControllerTests
    {
        [Fact]
        public async Task GetAllSuggestedCommentsAsync_ReturnsOkWithSuggestedComments()
        {
            var mock = new Mock<IRepository<SuggestedComment>>();
            mock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SuggestedComment>
            {
                new() {Id = It.IsAny<Guid>(), Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(),
                    CreatedDate = It.IsAny<DateTime>(), DiscussionId = It.IsAny<Guid>()},
                new() {Id = It.IsAny<Guid>(), Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(),
                    CreatedDate = It.IsAny<DateTime>(), DiscussionId = It.IsAny<Guid>()},
                new() {Id = It.IsAny<Guid>(), Content = It.IsAny<string>(), CreatedBy = It.IsAny<string>(),
                    CreatedDate = It.IsAny<DateTime>(), DiscussionId = It.IsAny<Guid>()}
            });
            var controller = new SuggestCommentController(mock.Object, new Mock<IRepository<Comment>>().Object,
                new Mock<IPublishEndpoint>().Object);

            var result = await controller.GetAllSuggestedCommentsAsync();

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
                new Mock<IPublishEndpoint>().Object);

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
            string userName = It.IsAny<string>();
            var mock = new Mock<IRepository<SuggestedComment>>();
            mock.Setup(x => x.GetByUserName(userName)).ReturnsAsync(new List<SuggestedComment>
            {
                new (){CreatedBy = userName}, new (){ CreatedBy = userName}
            });
            var controller = new SuggestCommentController(mock.Object, new Mock<IRepository<Comment>>().Object,
                new Mock<IPublishEndpoint>().Object);

            var result = await controller.GetSuggestedCommentsByUserNameAsync(userName);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var suggestedComments = Assert.IsType<List<SuggestedComment>>(methodResult.Value);
            Assert.Equal(2, suggestedComments.Count);
            mock.VerifyAll();
        }
    }
}
