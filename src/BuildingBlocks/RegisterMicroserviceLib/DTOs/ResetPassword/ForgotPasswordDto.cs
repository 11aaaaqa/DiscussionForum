using System.ComponentModel.DataAnnotations;

namespace RegisterMicroserviceLib.DTOs.ResetPassword
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
