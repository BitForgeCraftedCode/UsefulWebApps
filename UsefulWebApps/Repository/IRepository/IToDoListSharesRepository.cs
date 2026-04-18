using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IToDoListSharesRepository : IRepository<ToDoListShares>
    {
        //any ToDoListShares model specific database methods here
        Task<bool> ShareToDoList(long listId, string sharedWithUserId);
        Task<bool> UnshareToDoList(long listId);
        Task<List<ToDoLists>> GetToDoListsSharedWithUser(string userId);
        // Returns ListId -> list of friend DisplayNames this list is shared to
        Task<Dictionary<long, List<string>>> GetSharedToMapForOwner(string ownerUserId);
    }
}
