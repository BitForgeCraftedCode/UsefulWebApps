using UsefulWebApps.Models.ListBuddy;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IGroceryListRepository : IRepository<GroceryList>
    {
        //any GroceryList model specific database methods here
        Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemsAndCategories();
        Task GroceryListToggleComplete(int? id);
    }
}
