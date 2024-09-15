using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Discussion
{
    public class DiscussionDto
    {
        public string TopicName { get; set; }

        [Required]
        [StringLength(70, ErrorMessage = "Максимальная длина заголовка - 70 символов")]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required]
        [StringLength(1500, ErrorMessage = "Максимальная длина обсуждения - 1500 символов")]
        [Display(Name = "Обсуждение")]
        public string Content { get; set; }
    }
}
