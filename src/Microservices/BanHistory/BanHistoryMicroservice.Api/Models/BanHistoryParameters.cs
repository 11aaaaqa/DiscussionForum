namespace BanHistoryMicroservice.Api.Models
{
    public class BanHistoryParameters
    {
        private const int MaxPageSize = 30;
        public int PageNumber { get; set; } = 1;

        private int pageSize = 15;
        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
