using Microsoft.AspNetCore.Mvc.Rendering;

namespace UsefulWebApps.Models.ViewModels.Calendar
{
    public class ShareEventVM
    {
        public long EventId { get; set; }
        // Selected friend's UserId from the dropdown
        public string SelectedFriendUserId { get; set; } = string.Empty;

        // Populates the dropdown
        public IEnumerable<SelectListItem> FriendsList { get; set; } = new List<SelectListItem>();
    }
}
