using System.ComponentModel.DataAnnotations;

namespace Web.MVC.DTOs.Comment
{
    public class SuggestCommentDto
    {
        public string? CreatedBy { get; set; }

        [Required]
        [StringLength(800, ErrorMessage = "Максимальная длина комментария - 500 символов")]
        [Display(Name = "Оставить комментарий")]
        public string Content { get; set; }
    }
}
