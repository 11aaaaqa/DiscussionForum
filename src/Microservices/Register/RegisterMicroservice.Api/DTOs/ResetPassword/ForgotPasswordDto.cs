using System.ComponentModel.DataAnnotations;

namespace RegisterMicroservice.Api.DTOs.ResetPassword
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
