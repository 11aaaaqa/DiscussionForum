using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.User
{
    public class ChangeUserNameDto
    {
        [Required(ErrorMessage = "Поле \"Новое имя пользователя\" обязательно")]
        [StringLength(30, ErrorMessage = "Максимальная длина имени пользователя - 30 символов")]
        [Display(Name = "Новое имя пользователя")]
        [RegularExpression("^[a-zA-Z0-9_.]+$", ErrorMessage = "Имя может содержать только буквы английского алфавита, цифры, точки и нижние подчеркивания")]
        public string NewUserName { get; set; }

        public string? ReturnUrl { get; set; }
        public Guid UserId { get; set; }
    }
}
