using System.ComponentModel.DataAnnotations;

namespace UsefulWebApps.IdentityModels
{
    public class DeleteUserData
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string AdminUserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string AdminEmail { get; set; } = string.Empty;
    }
}
