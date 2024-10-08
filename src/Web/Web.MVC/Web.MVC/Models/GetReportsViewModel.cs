using Web.MVC.Models.ApiResponses.ReportResponses;

namespace Web.MVC.Models
{
    public class GetReportsViewModel
    {
        public List<ReportApiResponse> Reports { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NextPageNumber { get; set; }
        public int PreviousPageNumber { get; set; }
        public bool DoesNextPageExist { get; set; }
        public string? ReportType { get; set; }
    }
}
