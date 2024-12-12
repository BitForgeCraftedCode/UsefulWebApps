using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Please enter at least 10 characters.")]
        [Required(ErrorMessage = "Quote is required.")]
        public string Quote {  get; set; } = string.Empty;

        [Column("Author")]
        [ValidateNever]
        [StringLength(100, MinimumLength = 0)]
        public string Author { get; set; } = string.Empty;
    }
}
