using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Models.ViewModels.ListBuddy
{
    public class ToDoListVM
    {
        public ToDoLists ToDoList { get; set; }
        public List<ToDoItems> ToDoListItems { get; set; }

        public ToDoItems ToDoItem { get; set; }
    }
}
