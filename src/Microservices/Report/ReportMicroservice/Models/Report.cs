namespace ReportMicroservice.Models
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid IdReportedBy { get; set; }
        public Guid? CommentId { get; set; }
        public Guid? DiscussionId { get; set; }
        public string Reason { get; set; }
    }
}
