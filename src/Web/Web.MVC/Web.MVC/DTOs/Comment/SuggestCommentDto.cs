using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Comment
{
    public class SuggestCommentDto
    {
        public string? CreatedBy { get; set; }

        [Required]
        [Display(Name = "Оставить комментарий")]
        public string Content { get; set; }
    }
}
