using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository
{
    public class GroceryListRepository : Repository<GroceryList>, IGroceryListRepository
    {
        private readonly MySqlConnection _connection;
        public GroceryListRepository(MySqlConnection db) : base(db)
        {
            _connection = db;
        }

        //any GroceryList model specific database methods here
        public async Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemsAndCategories()
        {
            string query = @"
                    SELECT * FROM grocery_list;
                    SELECT * FROM grocery_categories;
                ";
            GridReader gridReader = await _connection.QueryMultipleAsync(query);
            List<GroceryList> groceryListItems = (List<GroceryList>)await gridReader.ReadAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            await _connection.CloseAsync();
            return (groceryListItems, groceryCategoriesEnum);
        }
       
        public async Task GroceryListToggleComplete(int? id)
        {
            await _connection.OpenAsync();
            //logic here
            await _connection.CloseAsync();
        }
    }
}
