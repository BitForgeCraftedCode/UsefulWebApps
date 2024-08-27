using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IGroceryListRepository : IRepository<GroceryList>
    {
        //any GroceryList model specific database methods here

        Task GroceryListToggleComplete(int? id);
    }
}
