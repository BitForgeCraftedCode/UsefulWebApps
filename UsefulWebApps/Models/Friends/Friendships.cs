using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.Friends
{
    [Table("friendships")]
    public class Friendships
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("RequesterUserId")]
        [Required]
        public string RequesterUserId { get; set; } = string.Empty;

        [Column("AddresseeUserId")]
        [Required]
        public string AddresseeUserId { get; set; } = string.Empty;

        [Column("Status")]
        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
    public enum FriendshipStatus : byte
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2
    }
}
