namespace BanHistoryMicroservice.Api.DTOs
{
    public class BanDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
        public uint DurationInDays { get; set; }
        public string BanType { get; set; }
    }
}
