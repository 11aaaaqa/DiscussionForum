namespace UserMicroservice.Api.DTOs
{
    public class BanDto
    {
        public string Reason { get; set; }
        public string BanType { get; set; }
        public uint ForDays { get; set; }
    }
}
