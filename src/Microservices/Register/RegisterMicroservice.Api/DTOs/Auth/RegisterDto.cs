using System.ComponentModel.DataAnnotations;

namespace RegisterMicroservice.Api.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Поле \"Эл. почта\" обязательно")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Эл. почта")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле \"Имя пользователя\" обязательно")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле \"Пароль\" обязательно")]
        [StringLength(int.MaxValue, ErrorMessage = "Пароль должен содержать как минимум 8 символов", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле \"Подтвердить пароль\" обязательно")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}

