using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.ListBuddy
{
    [Table("grocery_list_shares")]
    public class GroceryListShares
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("ListId")]
        [Required]
        public long ListId { get; set; }

        [Column("SharedWithUserId")]
        [Required]
        public string SharedWithUserId { get; set; } = string.Empty;
    }
}
