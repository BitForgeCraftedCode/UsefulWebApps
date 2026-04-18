using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IToDoListsRepository : IRepository<ToDoLists>
    {
        //any ToDoLists model specific database methods here
        Task<List<ToDoItems>> GetAllItemsInList(long? listId);

        Task<(ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListToggleComplete(long id, long listId);
    }
}
