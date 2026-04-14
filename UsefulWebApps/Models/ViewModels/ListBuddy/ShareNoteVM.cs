using Microsoft.AspNetCore.Mvc.Rendering;

namespace UsefulWebApps.Models.ViewModels.ListBuddy
{
    public class ShareNoteVM
    {
        // The note being shared
        public long NoteId { get; set; }
        public string NoteTitle { get; set; } = string.Empty;

        // Selected friend's UserId from the dropdown
        public string SelectedFriendUserId { get; set; } = string.Empty;

        // Populates the dropdown
        public IEnumerable<SelectListItem> FriendsList { get; set; } = new List<SelectListItem>();
    }
}
