using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using UsefulWebApps.Models.Friends;

namespace UsefulWebApps.Models.ViewModels.Friends
{
    public class EditUserProfileVM
    {
        public UserProfiles UserProfile { get; set; }
        [ValidateNever]
        public IFormFile ImageFile { get; set; }
    }
}
