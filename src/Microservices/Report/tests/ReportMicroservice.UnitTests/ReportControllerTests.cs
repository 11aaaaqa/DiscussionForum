using Microsoft.AspNetCore.Mvc;
using Moq;
using ReportMicroservice.Api.Controllers;
using ReportMicroservice.Api.Models;
using ReportMicroservice.Api.Services;

namespace ReportMicroservice.UnitTests
{
    public class ReportControllerTests
    {
        [Fact]
        public async Task GetAllReportsAsync_ReturnsOkWithListOfReports()
        {
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.GetAllReportsAsync()).ReturnsAsync(new List<Report>
            {
                new()
                {
                    Id = It.IsAny<Guid>(),Reason = It.IsAny<string>(), ReportedCommentContent = It.IsAny<string>(), UserIdReportedTo = It.IsAny<Guid>(),
                    UserNameReportedBy = It.IsAny<string>()
                },
                new()
                {
                    Id = It.IsAny<Guid>(),Reason = It.IsAny<string>(), ReportedCommentContent = It.IsAny<string>(), UserIdReportedTo = It.IsAny<Guid>(),
                    UserNameReportedBy = It.IsAny<string>()
                },
                new()
                {
                    Id = It.IsAny<Guid>(),Reason = It.IsAny<string>(), UserIdReportedTo = It.IsAny<Guid>(), UserNameReportedBy = It.IsAny<string>(),
                    ReportedDiscussionContent = It.IsAny<string>(), ReportedDiscussionTitle = It.IsAny<string>()
                }
            });
            var controller = new ReportController(mock.Object);

            var result = await controller.GetAllReportsAsync();

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var reports = Assert.IsType<List<Report>>(methodResult.Value);
            Assert.Equal(3, reports.Count);
            mock.Verify(x => x.GetAllReportsAsync());
        }
    }
}
