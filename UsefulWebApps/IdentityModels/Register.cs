using System.ComponentModel.DataAnnotations;

namespace UsefulWebApps.IdentityModels
{
    public class Register
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
