using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.DTO.ListBuddy
{
    public class ToDoListViewState
    {
        public ToDoLists ToDoList { get; set; }
        public List<ToDoItems> ListItems { get; set; }
    }
}
