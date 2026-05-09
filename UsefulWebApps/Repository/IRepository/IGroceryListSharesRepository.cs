using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IGroceryListSharesRepository : IRepository<GroceryListShares>
    {
        //any GroceryListShares model specific database methods here
        Task<bool> ShareGroceryList(long listId, string sharedWithUserId);
        Task<List<GroceryLists>> GetGroceryListsSharedWithUser(string userId);
        // Returns ListId -> list of friend DisplayNames this list is shared to
        Task<Dictionary<long, List<string>>> GetSharedToMapForOwner(string ownerUserId);
    }
}
