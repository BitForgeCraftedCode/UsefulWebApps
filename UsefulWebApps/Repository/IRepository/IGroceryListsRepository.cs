using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IGroceryListsRepository : IRepository<GroceryLists>
    {
        //any GroceryLists model specific database methods here
        Task<(List<GroceryListItems> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories)> GetAllItemsAndCategoriesInList(long? listId);
    }
}
