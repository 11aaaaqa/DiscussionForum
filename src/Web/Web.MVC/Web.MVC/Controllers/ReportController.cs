using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.Constants;
using Web.MVC.DTOs.Report;
using Web.MVC.Models;
using Web.MVC.Models.ApiRequests;
using Web.MVC.Models.ApiResponses.CustUserResponses;
using Web.MVC.Models.ApiResponses.ReportResponses;

namespace Web.MVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string url;

        public ReportController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.httpClientFactory = httpClientFactory;
            url = (string.IsNullOrEmpty(config["Url:Port"]))
                ? $"{config["Url:Protocol"]}://{config["Url:HostName"]}" : $"{config["Url:Protocol"]}://{config["Url:HostName"]}:{config["Url:Port"]}";
        }

        [Route("reports/add")]
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

        [Route("reports/add")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReport(CreateReportDto model)
        {
            if (ModelState.IsValid)
            {
                using HttpClient httpClient = httpClientFactory.CreateClient();

                var getLink =
                    $"{url}/api/profile/User/IsUserBannedByUserName/{User.Identity.Name}?banTypes[]={BanTypeConstants.GeneralBanType}&banTypes[]={BanTypeConstants.ReportBanType}";
                var isUserBannedResponse = await httpClient.GetAsync(getLink);
                if (!isUserBannedResponse.IsSuccessStatusCode) return View("ActionError");

                var isUserBanned = await isUserBannedResponse.Content.ReadFromJsonAsync<bool>();
                if (isUserBanned) return View("ReportBanned");

                var userResponse = await httpClient.GetAsync(
                    $"{url}/api/profile/User/GetUserByUserName/{model.ReportedUserName}");
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
                    $"{url}/api/Report/CreateReport", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return View("Thanks");
                }

                return View("ActionError");
            }

            return View(model);
        }

        [Route("reports")]
        [Authorize(Roles = UserRoleConstants.AdminRole + ", " + UserRoleConstants.ModeratorRole)]
        [HttpGet]
        public async Task<IActionResult> Reports(string reportType, int pageNumber, int pageSize)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                $"{url}/api/Report/GetReportsByReportType/{reportType}?pageNumber={pageNumber}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            var reports = await response.Content.ReadFromJsonAsync<List<ReportApiResponse>>();

            var doesNextPageExistResponse = await httpClient.GetAsync(
                $"{url}/api/Report/DoesNextPageByReportTypeExist?reportType={reportType}&pageSize={pageSize}&pageNumber={pageNumber + 1}");
            if (!doesNextPageExistResponse.IsSuccessStatusCode) return View("ActionError");

            var doesExist = await doesNextPageExistResponse.Content.ReadFromJsonAsync<bool>();

            return View(new GetReportsViewModel
            {
                PageSize = pageSize, CurrentPageNumber = pageNumber, NextPageNumber = pageNumber + 1, PreviousPageNumber = pageNumber - 1,
                Reports = reports, DoesNextPageExist = doesExist, ReportType = reportType
            });
        }

        [Route("reports/{reportId}")]
        [Authorize(Roles = UserRoleConstants.AdminRole + ", " + UserRoleConstants.ModeratorRole)]
        [HttpGet]
        public async Task<IActionResult> GetReport(Guid reportId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{url}/api/Report/GetReportById/{reportId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            ViewBag.ReturnUrl = returnUrl;
            var report = await response.Content.ReadFromJsonAsync<ReportApiResponse>();
            return View(report);
        }

        [Authorize(Roles = UserRoleConstants.AdminRole + ", " + UserRoleConstants.ModeratorRole)]
        [HttpPost]
        public async Task<IActionResult> DeleteReport(Guid reportId, string returnUrl)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();
            var response =
                await httpClient.DeleteAsync(
                    $"{url}/api/Report/DeleteReportById/{reportId}");
            if (!response.IsSuccessStatusCode) return View("ActionError");

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Reports", "Report", new {reportType = ReportTypeConstants.DiscussionType});
        }
    }
}
