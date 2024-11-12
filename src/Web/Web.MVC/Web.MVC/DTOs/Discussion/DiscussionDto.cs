using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Discussion
{
    public class DiscussionDto
    {
        public string TopicName { get; set; }

        [Required(ErrorMessage = "Поле \"Заголовок\" обязательно")]
        [StringLength(150, ErrorMessage = "Максимальная длина заголовка - 150 символов")]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Поле \"Обсуждение\" обязательно")]
        [StringLength(2000, ErrorMessage = "Максимальная длина обсуждения - 2000 символов")]
        [Display(Name = "Обсуждение")]
        public string Content { get; set; }
    }
}
