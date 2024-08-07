using System.ComponentModel.DataAnnotations;

namespace RegisterMicroserviceLib.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Поле \"Имя пользователя или адрес эл. почты\" обязательно")]
        [Display(Name = "Имя пользователя или адрес эл. почты")]
        public string UserNameOrEmail { get; set; }

        [Required(ErrorMessage = "Поле \"Пароль\" обязательно")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
