using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IToDoItemsRepository : IRepository<ToDoItems>
    {
        //any ToDoItems model specific database methods here
        Task<(bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListAddItem(ToDoItems toDoItem);
        Task<(bool success, bool wasConflict)> UpdateWithVersionCheck(ToDoItems toDoItem);
    }
}
