using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Models.ViewModels.ListBuddy
{
    public class MyGroceryListsVM
    {
        public List<GroceryLists> MyGroceryLists { get; set; }
        public List<GroceryLists> SharedWithMeGroceryLists { get; set; }
        // Key: ListId, Value: list of display names the list is shared with
        public Dictionary<long, List<string>> SharedToFriends { get; set; }
    }
}
