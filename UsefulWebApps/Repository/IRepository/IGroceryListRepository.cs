using UsefulWebApps.Models.ListBuddy;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IGroceryListRepository : IRepository<GroceryList>
    {
        //any GroceryList model specific database methods here
        Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemsAndCategories();
        Task<(GroceryList groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemAndCategoriesAtId(int? id);
        Task GroceryListToggleComplete(int? id);
    }
}
