using UsefulWebApps.DTO.ListBuddy;
using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IToDoItemsRepository : IRepository<ToDoItems>
    {
        //any ToDoItems model specific database methods here
        Task<(bool success, bool wasConflict, ToDoListViewState viewState)> ToDoListAddItem(ToDoItems toDoItem);
        Task<(bool success, bool wasConflict)> UpdateWithVersionCheck(ToDoItems toDoItem);
        Task<(bool success, bool wasConflict, ToDoListViewState viewState)> DeleteWithVersionCheck(long id, long listId, int expectedVersion);
    }
}
