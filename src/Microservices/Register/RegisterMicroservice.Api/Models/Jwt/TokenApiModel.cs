namespace RegisterMicroservice.Api.Models.Jwt
{
    public class TokenApiModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
