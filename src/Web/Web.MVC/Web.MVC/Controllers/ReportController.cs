using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;
using Web.MVC.DTOs.Report;
using Web.MVC.Models.ApiRequests;
using Web.MVC.Models.ApiResponses.CustUserResponses;
using Web.MVC.Models.ApiResponses.ReportResponses;

namespace Web.MVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ReportController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddReport(string reportedUserName, string? reportedCommentContent,
            string? reportedDiscussionTitle, string? reportedDiscussionContent, Guid? reportedDiscussionId, Guid reportedCommentId)
        {
            var model = new CreateReportDto
            {
                ReportedDiscussionId = reportedDiscussionId,
                ReportedCommentId = reportedCommentId,
                ReportedUserName = reportedUserName,
                ReportedCommentContent = reportedCommentContent,
                ReportedDiscussionContent = reportedDiscussionContent,
                ReportedDiscussionTitle = reportedDiscussionTitle
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReport(CreateReportDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient httpClient = httpClientFactory.CreateClient();

                var getLink =
                    $"http://user-microservice-api:8080/api/profile/User/IsUserBannedByUserName/{User.Identity.Name}?banTypes[]={BanTypeConstants.GeneralBanType}&banTypes[]={BanTypeConstants.ReportBanType}";
                var isUserBannedResponse = await httpClient.GetAsync(getLink);
                if (!isUserBannedResponse.IsSuccessStatusCode) return View("ActionError");

                var isUserBanned = await isUserBannedResponse.Content.ReadFromJsonAsync<bool>();
                if (isUserBanned) return View("ReportBanned");

                var userResponse = await httpClient.GetAsync(
                    $"http://user-microservice-api:8080/api/profile/User/GetUserByUserName/{model.ReportedUserName}");
                if (!userResponse.IsSuccessStatusCode) return View("ActionError");
                var user = await userResponse.Content.ReadFromJsonAsync<CustUserResponse>();

                var requestModel = new CreateReportRequest
                {
                    Reason = model.Reason, ReportedCommentContent = model.ReportedCommentContent, ReportedDiscussionContent = model.ReportedDiscussionContent,
                    ReportedDiscussionTitle = model.ReportedDiscussionTitle, UserIdReportedTo = user.Id, UserNameReportedBy = User.Identity.Name,
                    ReportedCommentId = model.ReportedCommentId, ReportedDiscussionId = model.ReportedDiscussionId
                };
                requestModel.ReportType = requestModel.ReportedCommentContent is null ? ReportTypeConstants.DiscussionType : ReportTypeConstants.CommentType;
                
                using StringContent jsonContent = new(JsonSerializer.Serialize(requestModel), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(
                    $"http://report-microservice-api:8080/api/Report/CreateReport", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return View("Thanks");
                }

                return View("ActionError");
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Reports(string reportType)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"http://report-microservice-api:8080/api/Report/GetReportsByReportType/{reportType}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var reports = await response.Content.ReadFromJsonAsync<List<ReportApiResponse>>();
            return View(reports);
        }

        [Route("reports/{reportId}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetReport(Guid reportId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"http://report-microservice-api:8080/api/Report/GetReportById/{reportId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            ViewBag.ReturnUrl = returnUrl;
            var report = await response.Content.ReadFromJsonAsync<ReportApiResponse>();
            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReport(Guid reportId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response =
                await httpClient.DeleteAsync(
                    $"http://report-microservice-api:8080/api/Report/DeleteReportById/{reportId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Reports", "Report", new {reportType = ReportTypeConstants.DiscussionType});
        }
    }
}
