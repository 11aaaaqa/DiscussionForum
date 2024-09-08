using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Report
{
    public class CreateReportDto
    {
        public string ReportedUserName { get; set; }
        public string? ReportedCommentContent { get; set; }
        public string? ReportedDiscussionTitle { get; set; }
        public string? ReportedDiscussionContent { get; set; }

        [Required]
        [Display(Name="Причина")]
        public string Reason { get; set; }
    }
}
