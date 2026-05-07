using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository
{
    public class GroceryListsRepository : Repository<GroceryLists>, IGroceryListsRepository
    {
        public GroceryListsRepository(MySqlConnection connection) : base(connection) { }
        //any GroceryLists model specific database methods here

        //return multiple types with a tuple https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples 
        public async Task<(List<GroceryListItems> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories)> GetAllItemsAndCategoriesInList(long? listId)
        {
            string sql = @"
                        SELECT * FROM grocery_list_items WHERE ListId = @listId ORDER BY SortOrder, Category, GroceryItem;
                        SELECT * FROM grocery_categories ORDER BY Category;
                        SELECT DISTINCT Category, SortOrder FROM grocery_list_items WHERE ListId = @listId ORDER BY SortOrder, Category;
                        ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sql, new { listId });
            List<GroceryListItems> groceryListItems = (List<GroceryListItems>)await gridReader.ReadAsync<GroceryListItems>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (List<UserGroceryCategories>)await gridReader.ReadAsync<UserGroceryCategories>();

            return (groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }
    }
}
