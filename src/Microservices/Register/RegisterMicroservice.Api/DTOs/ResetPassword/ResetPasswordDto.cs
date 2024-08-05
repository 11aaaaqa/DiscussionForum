using System.ComponentModel.DataAnnotations;

namespace RegisterMicroservice.Api.DTOs.ResetPassword
{
    public class ResetPasswordDto
    {
        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(int.MaxValue, ErrorMessage = "Пароль должен содержать как минимум 8 символов", MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }

        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
