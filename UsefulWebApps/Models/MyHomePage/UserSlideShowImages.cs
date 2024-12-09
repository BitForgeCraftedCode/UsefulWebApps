using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.MyHomePage
{
    [Table("user_slideshow_images")]
    public class UserSlideShowImages
    {
        [Key]
        [Column("UserSlideShowImageId")]
        public int UserSlideShowImageId { get; set; }

        [Column("UserId")]
        public string UserId { get; set; } = string.Empty;

        [Column("UserName")]
        public string UserName { get; set; } = string.Empty;

        [ForeignKey("SlideShowImageId")]
        [Column("SlideShowImageId")]
        public int SlideShowImageId { get; set; }
    }
}
