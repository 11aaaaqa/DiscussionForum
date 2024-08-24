namespace Web.MVC.DTOs.ResetPassword
{
    public class ForgotPasswordUri
    {
        public string Protocol { get; set; }
        public string DomainName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
