using UsefulWebApps.DTO.ListBuddy;
using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IGroceryListItemsRepository : IRepository<GroceryListItems>
    {
        //any GroceryListItems model specific database methods here
        Task<(GroceryLists groceryList, GroceryListItems groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemAndCategoriesAtId(long? id, long? listId);
        Task<(bool success, bool wasConflict)> GroceryListUpdate(GroceryListItems groceryListItem);
        Task<(bool success, bool wasConflict, GroceryListViewState viewState)> DeleteGroceryListItem(long? id, long? listId, int expectedVersion);
        Task<(bool success, bool wasConflict, GroceryListViewState viewState)> GroceryListAddItem(GroceryListItems groceryListItem);
        Task<bool> SaveUserGroceryListTemplate(string userId, long? listId);
        Task<(bool success, bool wasConflict)> UseSavedGroceryListTemplate(string userId, long? listId, int expectedVersion);
    }
}
