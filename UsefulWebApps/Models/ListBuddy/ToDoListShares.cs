using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.ListBuddy
{
    [Table("to_do_list_shares")]
    public class ToDoListShares
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
