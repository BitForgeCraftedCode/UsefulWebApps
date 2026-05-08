using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IGroceryListItemsRepository : IRepository<GroceryListItems>
    {
        //any GroceryListItems model specific database methods here
        Task<(GroceryListItems groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemAndCategoriesAtId(long? id);
        Task<(bool success, bool wasConflict)> GroceryListUpdate(GroceryListItems groceryListItem);
    }
}
