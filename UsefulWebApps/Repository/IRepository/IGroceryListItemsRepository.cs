using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IGroceryListItemsRepository : IRepository<GroceryListItems>
    {
        //any GroceryListItems model specific database methods here
        Task<(GroceryListItems groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemAndCategoriesAtId(long? id);
        Task<(bool success, bool wasConflict)> GroceryListUpdate(GroceryListItems groceryListItem);
        Task<(
            bool success, 
            bool wasConflict, 
            GroceryLists groceryList, 
            List<GroceryListItems> listItems, 
            IEnumerable<GroceryCategories> groceryCategoriesEnum, 
            List<UserGroceryCategories> userGroceryCategories)> DeleteGroceryListItem(long? id, long? listId, int expectedVersion);
        Task<(
            bool success,
            bool wasConflict,
            GroceryLists groceryList,
            List<GroceryListItems> listItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories)> GroceryListAddItem(GroceryListItems groceryListItem);
        Task<bool> SaveUserGroceryListTemplate(string userId, long? listId);
        Task<bool> UseSavedGroceryListTemplate(string userId, long? listId);
    }
}
