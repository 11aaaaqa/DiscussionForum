using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Topic
{
    public class TopicDto
    {
        [Required]
        [StringLength(40, ErrorMessage = "Максимальная длина названия - 40 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        public string SuggestedBy { get; set; } = string.Empty;
    }
}
