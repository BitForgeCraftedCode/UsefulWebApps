using UsefulWebApps.DTO.ListBuddy;
using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IToDoListsRepository : IRepository<ToDoLists>
    {
        //any ToDoLists model specific database methods here
        Task<List<ToDoItems>> GetAllItemsInList(long? listId);

        Task<(bool success, bool wasConflict, ToDoListViewState viewState)> ToDoListToggleComplete(long id, long listId, int expectedVersion);
        Task<(bool success, bool wasConflict, ToDoListViewState viewState)> ToDoListSortItem(long id, long listId, int sortOrder, int expectedVersion);
    }
}
