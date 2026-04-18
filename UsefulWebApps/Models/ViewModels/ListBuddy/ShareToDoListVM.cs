using Microsoft.AspNetCore.Mvc.Rendering;

namespace UsefulWebApps.Models.ViewModels.ListBuddy
{
    public class ShareToDoListVM
    {
        // The list being shared
        public long ListId { get; set; }
        public string ListTitle { get; set; }
        // Selected friend's UserId from the dropdown
        public string SelectedFriendUserId { get; set; } = string.Empty;

        // Populates the dropdown
        public IEnumerable<SelectListItem> FriendsList { get; set; } = new List<SelectListItem>();
    }
}
