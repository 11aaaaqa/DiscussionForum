using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
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

        [Fact]
        public async Task GetReportByIdAsync_ReturnsBadRequest()
        {
            var id = It.IsAny<Guid>();
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.GetReportByIdAsync(id)).ReturnsAsync((Report?)null);
            var controller = new ReportController(mock.Object);

            var result = await controller.GetReportByIdAsync(id);

            Assert.IsType<BadRequestResult>(result);
            mock.Verify(x => x.GetReportByIdAsync(id));
        }

        [Fact]
        public async Task GetReportByIdAsync_ReturnsOkWithReport()
        {
            var id = It.IsAny<Guid>();
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.GetReportByIdAsync(id)).ReturnsAsync(new Report { Id = id });
            var controller = new ReportController(mock.Object);

            var result = await controller.GetReportByIdAsync(id);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var report = Assert.IsType<Report>(methodResult.Value);
            Assert.Equal(id, report.Id);
            mock.Verify(x => x.GetReportByIdAsync(id));
        }
    }
}
