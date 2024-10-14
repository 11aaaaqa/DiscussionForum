using Web.MVC.Models.ApiResponses;

namespace Web.MVC.Models.ViewModels.AdminViewModels
{
    public class GetBansHistoryViewModel
    {
        public List<BanHistoryModel> Bans { get; set; }
        public bool DoesNextPageExist { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NextPageNumber { get; set; }
        public int PreviousPageNumber { get; set; }
        public string? SearchingQuery { get; set; }
    }
}
