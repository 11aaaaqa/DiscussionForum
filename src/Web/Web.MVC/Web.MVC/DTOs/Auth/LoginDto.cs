using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Поле \"Имя пользователя или адрес эл. почты\" обязательно")]
        [StringLength(80, ErrorMessage = "Максимальное количество символов почты превышено")]
        [Display(Name = "Имя пользователя или адрес эл. почты")]
        public string UserNameOrEmail { get; set; }

        [Required(ErrorMessage = "Поле \"Пароль\" обязательно")]
        [DataType(DataType.Password)]
        [StringLength(40, ErrorMessage = "Пароль должен содержать как минимум 8 символов", MinimumLength = 8)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
