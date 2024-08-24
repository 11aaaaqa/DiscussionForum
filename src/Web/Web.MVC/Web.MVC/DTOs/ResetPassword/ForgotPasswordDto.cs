using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.ResetPassword
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ForgotPasswordUri? Uri { get; set; }
    }
}
