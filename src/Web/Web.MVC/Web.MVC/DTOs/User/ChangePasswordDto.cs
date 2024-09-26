using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.User
{
    public class ChangePasswordDto
    {
        public Guid? UserId { get; set; }

        [Required(ErrorMessage = "Поле \"Старый пароль\" обязательно")]
        [StringLength(40)]
        [DataType(DataType.Password)]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Поле \"Новый пароль\" обязательно")]
        [StringLength(40, ErrorMessage = "Пароль должен содержать как минимум 8 символов", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }
    }
}
