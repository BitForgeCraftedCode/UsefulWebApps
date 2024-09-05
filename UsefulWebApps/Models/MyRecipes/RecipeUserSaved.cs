using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.MyRecipes
{
    [Table("receip_usersaved")]
    public class RecipeUserSaved
    {
        [Key]
        [Column("UserSavedId")]
        public int UserSavedId { get; set; }

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
