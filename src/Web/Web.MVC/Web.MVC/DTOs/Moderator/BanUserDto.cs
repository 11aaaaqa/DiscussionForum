using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Moderator
{
    public class BanUserDto
    {
        [Required(ErrorMessage = "Поле \"Длительность в днях\" обязательно")]
        [Display(Name = "Длительность в днях")]
        public uint ForDays { get; set; }

        public string? BanType { get; set; }
        public string? BannedBy { get; set; }

        [Required(ErrorMessage = "Поле \"Причина\" обязательно")]
        [StringLength(100, ErrorMessage = "Максимальная длина поля \"Причина\" 100 символов")]
        [Display(Name = "Причина")]
        public string Reason { get; set; }
    }
}
