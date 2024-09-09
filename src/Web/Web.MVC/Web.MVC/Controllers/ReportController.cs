using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.MVC.DTOs.Report;
using Web.MVC.Models.ApiRequests;
using Web.MVC.Models.ApiResponses.CustUserResponses;

namespace Web.MVC.Controllers
{
    public class ReportController : Controller
    {
        private const string DiscussionType = "Обсуждение";
        private const string CommentType = "Комментарий";
        private readonly IHttpClientFactory httpClientFactory;

        public ReportController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddReport(string reportedUserName, string? reportedCommentContent,
            string? reportedDiscussionTitle, string? reportedDiscussionContent)
        {
            var model = new CreateReportDto
            {
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

                var userResponse = await httpClient.GetAsync(
                    $"http://user-microservice-api:8080/api/profile/User/GetUserByUserName/{model.ReportedUserName}");
                if (!userResponse.IsSuccessStatusCode) return View("ActionError");
                var user = await userResponse.Content.ReadFromJsonAsync<CustUserResponse>();

                var requestModel = new CreateReportRequest
                {
                    Reason = model.Reason, ReportedCommentContent = model.ReportedCommentContent, ReportedDiscussionContent = model.ReportedDiscussionContent,
                    ReportedDiscussionTitle = model.ReportedDiscussionTitle, UserIdReportedTo = user.Id, UserNameReportedBy = User.Identity.Name
                };
                requestModel.ReportType = requestModel.ReportedCommentContent is null ? DiscussionType : CommentType;
                
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
    }
}
