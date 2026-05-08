using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Models.ViewModels.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository
{
    public class GroceryListItemsRepository : Repository<GroceryListItems>, IGroceryListItemsRepository
    {
        public GroceryListItemsRepository(MySqlConnection connection) : base(connection) { }

        //any GroceryListItems model specific database methods here

        public async Task<(GroceryListItems groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemAndCategoriesAtId(long? id)
        {
            string sql = @"
                SELECT * FROM grocery_list_items WHERE Id = @id;
                SELECT * FROM grocery_categories ORDER BY Category;
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sql, new { id });
            GroceryListItems groceryListItem = await gridReader.ReadSingleAsync<GroceryListItems>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            return (groceryListItem, groceryCategoriesEnum);
        }

        //transaction method
        public async Task<(bool success, bool wasConflict)> GroceryListUpdate(GroceryListItems groceryListItem)
        {
            // Version check + bump
            string sqlBump = @"UPDATE grocery_lists SET Version = Version + 1 
                       WHERE Id = @listId AND Version = @expectedVersion";
            int bumped = await _connection.ExecuteAsync(sqlBump,
                new { listId = groceryListItem.ListId, expectedVersion = groceryListItem.ListVersion }, transaction: _transaction);
            // conflict
            if (bumped == 0) return (false, true);

            //before update get the sort order of the category if no category yet set sort order to 1
            string sql = "SELECT SortOrder FROM grocery_list_items WHERE Category = @category AND ListId = @listId";
            int? sortOrder = await _connection.QueryFirstOrDefaultAsync<int?>(sql, new { category = groceryListItem.Category, listId = groceryListItem.ListId }, transaction: _transaction);
            if (sortOrder == null)
            {
                sortOrder = 1;
            }
            string updateSql = @"UPDATE grocery_list_items SET GroceryItem = @groceryItem, Category = @category, Complete = @complete, SortOrder = @sortOrder WHERE Id = @id";
            int rowCount = await _connection.ExecuteAsync(updateSql, new
            {
                groceryItem = groceryListItem.GroceryItem,
                category = groceryListItem.Category,
                complete = groceryListItem.Complete,
                sortOrder = sortOrder,
                id = groceryListItem.Id
            }, transaction: _transaction);

            // failed to update but no conflict
            if (rowCount == 0) return (false, false);

            return (true, false);
        }

    }
}
