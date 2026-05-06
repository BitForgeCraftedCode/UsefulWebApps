using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.ListBuddy
{
    [Table("grocery_lists")]
    public class GroceryLists
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("UserId")]
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Column("ListTitle")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please Enter At Least 3 Characters.")]
        [Required(ErrorMessage = "List Title Is Required.")]
        public string ListTitle { get; set; } = string.Empty;

        [Column("Version")]
        public int Version { get; set; } = 0;

        [Column("CreatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
