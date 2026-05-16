using UsefulWebApps.DTO.ListBuddy;
using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IGroceryListsRepository : IRepository<GroceryLists>
    {
        //any GroceryLists model specific database methods here
        Task<IEnumerable<GroceryCategories>> GetGroceryCategoriesEnum();
        Task<GroceryListViewState> GetAllItemsAndCategoriesInList(long? listId);
        Task<(bool success, bool wasConflict, GroceryListViewState viewState)> GroceryListToggleComplete(long? id, long? listId, int expectedVersion);
        Task<(bool success, bool wasConflict, GroceryListViewState viewState)> GroceryListSortCategories(long? listId, int newSortOrder, int expectedVersion, string category);
    }
}
