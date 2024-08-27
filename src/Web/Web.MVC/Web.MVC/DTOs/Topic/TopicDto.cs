using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Topic
{
    public class TopicDto
    {
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }
}
