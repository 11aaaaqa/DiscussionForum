using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Поле \"Эл. почта\" обязательно")]
        [StringLength(80, ErrorMessage = "Максимальное количество символов почты превышено")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Эл. почта")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле \"Имя пользователя\" обязательно")]
        [StringLength(30, ErrorMessage = "Максимальная длина имени пользователя - 30 символов")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле \"Пароль\" обязательно")]
        [StringLength(40, ErrorMessage = "Пароль должен содержать как минимум 8 символов", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле \"Подтвердить пароль\" обязательно")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }

        public ConfirmEmailMethodUri? Uri { get; set; }
    }
}
