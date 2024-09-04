using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UsefulWebApps.Models.MyRecipes
{
    [Table("recipe_comments")]
    public class RecipeComment
    {
        [Key]
        [Column("CommentId")]
        public int CommentId { get; set; }

        [Column("Comment")]
        [Required(ErrorMessage = "Recipe comment is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Please enter at least 10 characters.")]
        public string Comment { get; set; } = string.Empty;

        [Column("UserId")]
        [Required(ErrorMessage = "User Id is required.")]
        public string UserId { get; set; } = string.Empty;

        [Column("UserName")]
        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; } = string.Empty;

        [ForeignKey("RecipeId")]
        [Column("RecipeId")]
        public int RecipeId { get; set; }
    }
}
