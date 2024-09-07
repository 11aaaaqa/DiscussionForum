using Microsoft.AspNetCore.Mvc;
using ReportMicroservice.Api.DTOs;
using ReportMicroservice.Api.Models;
using ReportMicroservice.Api.Services;

namespace ReportMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService<Report> reportService;

        public ReportController(IReportService<Report> reportService)
        {
            this.reportService = reportService;
        }

        [Route("GetAllReports")]
        [HttpGet]
        public async Task<IActionResult> GetAllReportsAsync() => Ok(await reportService.GetAllReportsAsync());

        [Route("GetReportById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetReportByIdAsync(Guid id)
        {
            var report = await reportService.GetReportByIdAsync(id);
            if (report is null)
                return BadRequest();
            
            return Ok(report);
        }

        [Route("CreateReport")]
        [HttpPost]
        public async Task<IActionResult> CreateReportAsync([FromBody]CreateReportDto model)
        {
            var report = await reportService.CreateReportAsync(new Report
            {
                Id = Guid.NewGuid(),
                Reason = model.Reason,
                ReportedCommentContent = model.ReportedCommentContent,
                ReportedDiscussionContent = model.ReportedDiscussionContent,
                ReportedDiscussionTitle = model.ReportedDiscussionTitle,
                UserIdReportedTo = model.UserIdReportedTo,
                UserNameReportedBy = model.UserNameReportedBy
            });
            return Ok(report);
        }

        [Route("DeleteReportsByUserName/{userName}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteReportsByUserNameAsync(string userName)
        {
            await reportService.DeleteReportsByUserName(userName);
            return Ok();
        }
    }
}
