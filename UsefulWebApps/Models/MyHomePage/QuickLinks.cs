using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.MyHomePage
{
    [Table("quick_links")]
    public class QuickLinks
    {
        [Key]
        [Column("QuickLinkId")]
        public int QuickLinkId { get; set; }

        [Column("ImagePath")]
        public string ImagePath { get; set; } = string.Empty;

        [Column("URL")]
        public string URL { get; set; } = string.Empty;

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Category")]
        public string Category {  get; set; } = string.Empty;   

    }
}
