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
        }
    }
}
