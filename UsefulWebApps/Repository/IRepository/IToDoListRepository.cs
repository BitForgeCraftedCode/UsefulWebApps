using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IToDoListRepository : IRepository<ToDoList>
    {
        //any ToDoList model specific database methods here
        Task<List<ToDoList>> ToDoListToggleComplete(int? id, string userId);
        Task<List<ToDoList>> ToDoListAdd(ToDoList toDoList);
        Task<List<string>> GetMyToDoLists(string userId);
    }
}
