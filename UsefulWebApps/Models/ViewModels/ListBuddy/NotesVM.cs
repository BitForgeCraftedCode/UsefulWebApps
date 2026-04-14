using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Models.ViewModels.ListBuddy
{
    public class NotesVM
    {
        public List<Notes> MyNotes { get; set; }
        public List<Notes> SharedWithMeNotes { get; set; }
        // Key: NoteId, Value: list of display names the note is shared with
        public Dictionary<int, List<string>> SharedToFriends { get; set; }
    }
}
