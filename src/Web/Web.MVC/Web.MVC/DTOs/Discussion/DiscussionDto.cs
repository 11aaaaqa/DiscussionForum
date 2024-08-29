using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Discussion
{
    public class DiscussionDto
    {
        public string TopicName { get; set; }

        [Required]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Обсуждение")]
        public string Content { get; set; }
    }
}
