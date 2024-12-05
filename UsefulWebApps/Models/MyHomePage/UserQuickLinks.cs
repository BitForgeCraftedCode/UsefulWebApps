using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UsefulWebApps.Models.MyHomePage
{
    [Table("user_quick_links")]
    public class UserQuickLinks
    {
        [Key]
        [Column("UserQuickLinkId")]
        public int UserQuickLinkId { get; set; }

        [Column("UserId")]
        public string UserId { get; set; } = string.Empty;

        [Column("UserName")]
        public string UserName { get; set; } = string.Empty;

        [ForeignKey("QuickLinkId")]
        [Column("QuickLinkId")]
        public int QuickLinkId { get; set; }
    }
}
