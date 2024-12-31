using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.Weather
{
    [Table("locations")]
    public class Locations
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("City")]
        [Required(ErrorMessage = "City Is Required")]
        public string City { get; set; } = string.Empty;

        [Column("Latitude")]
        [ValidateNever]
        public double Latitude { get; set; }

        [Column("Longitude")]
        [ValidateNever]
        public double Longitude { get; set; }

        [Column("Country")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Please Enter 2 Character ISO Country Code.")]
        [Required(ErrorMessage = "Country Is Required")]
        public string Country { get; set; } = string.Empty;

        [Column("State")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Please Enter 2 Character State Abbreviation. Outside US enter NA")]
        [Required(ErrorMessage = "State Is Required. Outside US enter NA")]
        public string State { get; set; } = string.Empty;

        [Column("Zip")]
        [ValidateNever]
        public string Zip { get; set; } = string.Empty;

        [Column("UserId")]
        public string UserId { get; set; } = string.Empty;

        [Column("IsDefault")]
        public bool IsDefault { get; set; }

          
    }
}
