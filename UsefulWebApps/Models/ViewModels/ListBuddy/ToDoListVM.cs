using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Models.ViewModels.ListBuddy
{
    public class ToDoListVM
    {
        public ToDoList ToDoList { get; set; }
        public List<ToDoList> ToDoListItems { get; set; }
    }
}
