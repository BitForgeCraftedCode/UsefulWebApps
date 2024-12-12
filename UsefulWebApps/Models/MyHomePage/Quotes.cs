using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.MyHomePage
{
    [Table("quotes")]
    public class Quotes
    {
        [Key]
        [Column("QuoteId")]
        public int QuoteId { get; set; }

        [Column("Quote")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        [Required(ErrorMessage = "Quote is required.")]
        public string Quote {  get; set; } = string.Empty;

        [Column("Author")]
        public string Author { get; set; } = string.Empty;
    }
}
