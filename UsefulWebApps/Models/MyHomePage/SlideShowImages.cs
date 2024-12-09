using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.MyHomePage
{
    [Table("slideshow_images")]
    public class SlideShowImages
    {
        [Key]
        [Column("SlideShowImageId")]
        public int SlideShowImageId { get; set; }

        [Column("ImagePath")]
        public string ImagePath { get; set; } = string.Empty;

        [Column("FolderName")]
        public string FolderName { get; set; } = string.Empty;

    }
}
