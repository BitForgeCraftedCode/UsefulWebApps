using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Models.ViewModels.ListBuddy
{
    public class MyToDoListsVM
    {
        public List<ToDoLists> MyToDoLists { get; set; }
        public List<ToDoLists> SharedWithMeToDoLists { get; set; }
        // Key: ListId, Value: list of display names the note is shared with
        public Dictionary<long, List<string>> SharedToFriends { get; set; }
    }
}
