using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Report
{
    public class CreateReportDto
    {
        public string ReportedUserName { get; set; }
        public string? ReportedCommentContent { get; set; }
        public string? ReportedDiscussionTitle { get; set; }
        public string? ReportedDiscussionContent { get; set; }
        public Guid? ReportedCommentId { get; set; }
        public Guid? ReportedDiscussionId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Максимальная длина причины - 50 символов")]
        [Display(Name="Причина")]
        public string Reason { get; set; }
    }
}
