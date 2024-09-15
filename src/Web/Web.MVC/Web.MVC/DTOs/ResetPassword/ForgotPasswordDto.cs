using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.ResetPassword
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Эл. почта")]
        [StringLength(80, ErrorMessage = "Максимальное количество символов почты превышено")]
        public string Email { get; set; }

        public ForgotPasswordUri? Uri { get; set; }
    }
}
