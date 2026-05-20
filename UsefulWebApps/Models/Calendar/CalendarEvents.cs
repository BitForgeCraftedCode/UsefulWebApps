using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.Calendar
{
    [Table("calendar_events")]
    public class CalendarEvents
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("UserId")]
        public string? UserId { get; set; } = string.Empty;

        [Column("Title")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Please Enter At Least 3 Characters.")]
        [Required(ErrorMessage = "Event Title Is Required.")]
        public string Title { get; set; } = string.Empty;

        [Column("Description")]
        //allow null empty Description
        [ValidateNever]
        [StringLength(5000, MinimumLength = 0)]
        public string Description { get; set; } = string.Empty;

        [Column("StartDate")]
        [Required]
        public DateTime StartDate { get; set; }

        [Column("EndDate")]
        [Required]
        public DateTime EndDate { get; set; }

        [Column("IsAllDay")]
        [Required]
        public bool IsAllDay { get; set; } = false;

        [Column("RRule")]
        public string? RRule { get; set; } = string.Empty;

        //RDate adds extra occurrences that don't match the RRULE -- INCLUDE
        [Column("RDate")]
        public string? RDate { get; set; } = string.Empty;

        //ExDate removes specific occurrences that would normally happen -- EXCLUDE
        [Column("ExDate")]
        public string? ExDate { get; set; } = string.Empty;

        [Column("IsPrivate")]
        [Required]
        public bool IsPrivate { get; set; } = false;

    }
}
