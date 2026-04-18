using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IToDoItemsRepository : IRepository<ToDoItems>
    {
        //any ToDoItems model specific database methods here
        Task<(ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListAddItem(ToDoItems toDoItem);
    }
}
