namespace RegisterMicroservice.Api.DTOs.Auth
{
    public class ConfirmEmailMethodUri
    {
        public string Protocol { get; set; }
        public string DomainName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
