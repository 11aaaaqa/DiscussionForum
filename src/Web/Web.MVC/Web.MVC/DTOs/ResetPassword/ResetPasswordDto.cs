using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.ResetPassword
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Поле \"Новый пароль\" обязательно")]
        [Display(Name = "Новый пароль")]
        [DataType(DataType.Password)]
        [StringLength(40, ErrorMessage = "Пароль должен содержать как минимум 8 символов", MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле \"Подтвердите пароль\" обязательно")]
        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [StringLength(40, ErrorMessage = "Пароль должен содержать как минимум 8 символов", MinimumLength = 8)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }

        public string? UserId { get; set; }
        public string? Token { get; set; }
    }
}
