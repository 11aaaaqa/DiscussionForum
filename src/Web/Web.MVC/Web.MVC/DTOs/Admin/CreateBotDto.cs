using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Admin
{
    public class CreateBotDto
    {
        [Required(ErrorMessage = "Поле \"Имя пользователя\" обязательно")]
        [StringLength(30, ErrorMessage = "Максимальная длина имени пользователя - 30 символов")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле \"Эл. почта\" обязательно")]
        [StringLength(80, ErrorMessage = "Максимальное количество символов почты превышено")]
        [Display(Name = "Эл. почта")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле \"Пароль\" обязательно")]
        [StringLength(40, ErrorMessage = "Пароль должен содержать как минимум 8 символов", MinimumLength = 8)]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле \"Подтвердите пароль\" обязательно")]
        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
    }
}
