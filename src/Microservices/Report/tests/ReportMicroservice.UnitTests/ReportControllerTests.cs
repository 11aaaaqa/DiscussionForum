using Microsoft.AspNetCore.Mvc;
using Moq;
using ReportMicroservice.Api.Controllers;
using ReportMicroservice.Api.DTOs;
using ReportMicroservice.Api.Models;
using ReportMicroservice.Api.Services;

namespace ReportMicroservice.UnitTests
{
    public class ReportControllerTests
    {
        [Fact]
        public async Task GetAllReportsAsync_ReturnsOkWithListOfReports()
        {
            var reportParameters = new ReportParameters { PageNumber = 1, PageSize = 3 };
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.GetAllReportsAsync(reportParameters)).ReturnsAsync(new List<Report>
            {
                new()
                {
                    Id = It.IsAny<Guid>(),Reason = It.IsAny<string>(), ReportedCommentContent = It.IsAny<string>(), UserIdReportedTo = It.IsAny<Guid>(),
                    UserNameReportedBy = It.IsAny<string>(), ReportType = It.IsAny<string>(), ReportedCommentId = It.IsAny<Guid>()
                },
                new()
                {
                    Id = It.IsAny<Guid>(),Reason = It.IsAny<string>(), ReportedCommentContent = It.IsAny<string>(), UserIdReportedTo = It.IsAny<Guid>(),
                    UserNameReportedBy = It.IsAny<string>(), ReportType = It.IsAny<string>(), ReportedCommentId = It.IsAny<Guid>()
                },
                new()
                {
                    Id = It.IsAny<Guid>(),Reason = It.IsAny<string>(), UserIdReportedTo = It.IsAny<Guid>(), UserNameReportedBy = It.IsAny<string>(),
                    ReportedDiscussionContent = It.IsAny<string>(), ReportedDiscussionTitle = It.IsAny<string>(),
                    ReportType = It.IsAny<string>(), ReportedDiscussionId = It.IsAny<Guid>()
                }
            });
            var controller = new ReportController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetAllReportsAsync(reportParameters);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var reports = Assert.IsType<List<Report>>(methodResult.Value);
            Assert.Equal(3, reports.Count);
            mock.Verify(x => x.GetAllReportsAsync(reportParameters));
        }

        [Fact]
        public async Task GetReportByIdAsync_ReturnsBadRequest()
        {
            var id = It.IsAny<Guid>();
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.GetReportByIdAsync(id)).ReturnsAsync((Report?)null);
            var controller = new ReportController(mock.Object, new Mock<IPaginationService>().Object);

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
            var controller = new ReportController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetReportByIdAsync(id);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var report = Assert.IsType<Report>(methodResult.Value);
            Assert.Equal(id, report.Id);
            mock.Verify(x => x.GetReportByIdAsync(id));
        }

        [Fact]
        public async Task DeleteReportsByUserNameAsync_ReturnsOk()
        {
            string userName = It.IsAny<string>();
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.DeleteReportsByUserName(userName));
            var controller = new ReportController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.DeleteReportsByUserNameAsync(userName);

            Assert.IsType<OkResult>(result);
            mock.Verify(x => x.DeleteReportsByUserName(userName));
        }

        [Fact]
        public async Task CreateReportAsync_ReturnsOkWithReport()
        {
            var model = new CreateReportDto
            {
                Reason = It.IsAny<string>(),
                ReportedCommentContent = It.IsAny<string>(),
                UserIdReportedTo = It.IsAny<Guid>(),
                UserNameReportedBy = It.IsAny<string>()
            };
            var mock = new Mock<IReportService<Report>>();
            var controller = new ReportController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.CreateReportAsync(model);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetReportsByReportTypeAsync_ReturnsBadRequest()
        {
            var reportType = It.IsAny<string>();
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.GetReportsByReportType(reportType)).ReturnsAsync((List<Report>?)null);
            var controller = new ReportController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetReportsByReportTypeAsync(reportType);

            Assert.IsType<BadRequestResult>(result);
            mock.Verify(x => x.GetReportsByReportType(reportType));
        }

        [Fact]
        public async Task GetReportsByReportTypeAsync_ReturnsOkWithListOfReports()
        {
            var reportType = It.IsAny<string>();
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.GetReportsByReportType(reportType)).ReturnsAsync(new List<Report>
            {
                new()
                {
                    ReportType = reportType, Id = Guid.NewGuid(), Reason = It.IsAny<string>(), UserIdReportedTo = Guid.NewGuid(),
                    UserNameReportedBy = It.IsAny<string>(), ReportedCommentContent = It.IsAny<string>()
                },
                new()
                {
                    ReportType = reportType, Id = Guid.NewGuid(), Reason = It.IsAny<string>(), UserIdReportedTo = Guid.NewGuid(),
                    UserNameReportedBy = It.IsAny<string>(), ReportedDiscussionContent = It.IsAny<string>(), ReportedDiscussionTitle = It.IsAny<string>()
                }
            });
            var controller = new ReportController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetReportsByReportTypeAsync(reportType);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var reports = Assert.IsType<List<Report>>(methodResult.Value);
            Assert.Equal(2, reports.Count);
            mock.Verify(x => x.GetReportsByReportType(reportType));
        }

        [Fact]
        public async Task GetReportsByReportTypeAsync_ReturnsOkWithListOfReportsThatContainsZeroObjects()
        {
            var reportType = It.IsAny<string>();
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.GetReportsByReportType(reportType)).ReturnsAsync(new List<Report>());
            var controller = new ReportController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetReportsByReportTypeAsync(reportType);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, methodResult.StatusCode);
            Assert.NotNull(methodResult.Value);
            var reports = Assert.IsType<List<Report>>(methodResult.Value);
            Assert.Empty(reports);
            mock.Verify(x => x.GetReportsByReportType(reportType));
        }

        [Fact]
        public async Task DeleteReportByIdAsync_ReturnsOk()
        {
            var reportId = It.IsAny<Guid>();
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.DeleteReportById(reportId));
            var controller = new ReportController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.DeleteReportByIdAsync(reportId);

            Assert.IsType<OkResult>(result);
            mock.Verify(x =>x.DeleteReportById(reportId));
        }

        [Fact]
        public async Task GetReportsByUserNameAsync_ReturnsOkWithListIfReportsWithSpecifiedCreatedByUserName()
        {
            var userName = It.IsAny<string>();
            var mock = new Mock<IReportService<Report>>();
            mock.Setup(x => x.GetByUserNameAsync(userName)).ReturnsAsync(new List<Report>
            {
                new(){UserNameReportedBy = userName}, new(){UserNameReportedBy = userName},new(){UserNameReportedBy = userName}
            });
            var controller = new ReportController(mock.Object, new Mock<IPaginationService>().Object);

            var result = await controller.GetReportsByUserNameAsync(userName);

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            var reports = Assert.IsType<List<Report>>(methodResult.Value);
            Assert.Equal(3, reports.Count);
            mock.Verify(x => x.GetByUserNameAsync(userName));
        }

        [Fact]
        public async Task DoesNextPageExistAsync_ReturnsOkWithTrue()
        {
            int pageSize = 20;
            int pageNumber = 5;
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextReportsPageExistAsync(pageSize, pageNumber)).ReturnsAsync(true);
            var controller = new ReportController(new Mock<IReportService<Report>>().Object, mock.Object);

            var result = await controller.DoesNextPageExistAsync(new ReportParameters
                { PageSize = pageSize, PageNumber = pageNumber });

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.True(doesExist);
        }

        [Fact]
        public async Task DoesNextPageExistAsync_ReturnsOkWithFalse()
        {
            int pageSize = 20;
            int pageNumber = 5;
            var mock = new Mock<IPaginationService>();
            mock.Setup(x => x.DoesNextReportsPageExistAsync(pageSize, pageNumber)).ReturnsAsync(false);
            var controller = new ReportController(new Mock<IReportService<Report>>().Object, mock.Object);

            var result = await controller.DoesNextPageExistAsync(new ReportParameters
                { PageSize = pageSize, PageNumber = pageNumber });

            var methodResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(methodResult.Value);
            bool doesExist = Assert.IsType<bool>(methodResult.Value);
            Assert.False(doesExist);
        }
    }
}
