namespace RegisterMicroservice.Api.Models.ApiResponses
{
    public class GetUserResponse
    {
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string Id { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
