using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IGroceryListsRepository : IRepository<GroceryLists>
    {
        //any GroceryLists model specific database methods here
        Task<IEnumerable<GroceryCategories>> GetGroceryCategoriesEnum();
        Task<(List<GroceryListItems> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories)> GetAllItemsAndCategoriesInList(long? listId);
        Task<(
            bool success, 
            bool wasConflict, 
            GroceryLists groceryList, 
            List<GroceryListItems> listItems, 
            IEnumerable<GroceryCategories> groceryCategoriesEnum, 
            List<UserGroceryCategories> userGroceryCategories)> GroceryListToggleComplete(long? id, long? listId, int expectedVersion);
        Task<(
            bool success,
            bool wasConflict,
            GroceryLists groceryList,
            List<GroceryListItems> listItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories)> GroceryListSortCategories(long? listId, int newSortOrder, int expectedVersion, string category);
    }
}
