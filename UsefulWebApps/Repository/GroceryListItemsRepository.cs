using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
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

        //transaction method
        public async Task<(
            bool success,
            bool wasConflict,
            GroceryLists groceryList,
            List<GroceryListItems> listItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories)> DeleteGroceryListItem(long? id, long? listId, int expectedVersion)
        {
            // Version check + bump
            string sqlBump = @"UPDATE grocery_lists SET Version = Version + 1 
                       WHERE Id = @listId AND Version = @expectedVersion";
            int bumped = await _connection.ExecuteAsync(sqlBump, new { listId, expectedVersion }, transaction: _transaction);
            if (bumped == 0)
            {
                string sql = @"
                        SELECT * FROM grocery_lists WHERE Id = @listId;
                        SELECT * FROM grocery_list_items WHERE ListId = @listId ORDER BY SortOrder, Category, GroceryItem;
                        SELECT * FROM grocery_categories ORDER BY Category;
                        SELECT DISTINCT Category, SortOrder FROM grocery_list_items WHERE ListId = @listId ORDER BY SortOrder, Category;
                        ";
                GridReader gridReader = await _connection.QueryMultipleAsync(sql, new { listId }, transaction: _transaction);
                GroceryLists current = await gridReader.ReadSingleAsync<GroceryLists>();
                List<GroceryListItems> currentItems = (List<GroceryListItems>)await gridReader.ReadAsync<GroceryListItems>();
                IEnumerable<GroceryCategories> currentGroceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
                List<UserGroceryCategories> currentUserGroceryCategories = (List<UserGroceryCategories>)await gridReader.ReadAsync<UserGroceryCategories>();

                return (false, true, current, currentItems, currentGroceryCategoriesEnum, currentUserGroceryCategories);
            }
            string deleteSql = @"DELETE FROM grocery_list_items WHERE Id = @id";
            await _connection.ExecuteAsync(deleteSql, new { id }, transaction: _transaction);

            //get row count - distinct categories
            string countSql = @"SELECT COUNT(DISTINCT Category) FROM grocery_list_items WHERE ListId = @listId";
            int rowCount = await _connection.QuerySingleAsync<int>(countSql, new { listId }, transaction: _transaction);
            int newMax = rowCount == 0 ? 0 : rowCount;

            if (rowCount > 0)
            {
                //Ensure NO SortOrder value can exceed the maximum valid index after delete
                string sqlRenormalize = @"UPDATE grocery_list_items SET SortOrder = LEAST(SortOrder, @newMax) WHERE ListId = @ListId;";
                await _connection.ExecuteAsync(sqlRenormalize, new { newMax, listId }, transaction: _transaction);
            }

            string sql2 = @"
                        SELECT * FROM grocery_lists WHERE Id = @listId;
                        SELECT * FROM grocery_list_items WHERE ListId = @listId ORDER BY SortOrder, Category, GroceryItem;
                        SELECT * FROM grocery_categories ORDER BY Category;
                        SELECT DISTINCT Category, SortOrder FROM grocery_list_items WHERE ListId = @listId ORDER BY SortOrder, Category;
                        ";
            GridReader gridReader2 = await _connection.QueryMultipleAsync(sql2, new { listId }, transaction: _transaction);
            GroceryLists groceryList = await gridReader2.ReadSingleAsync<GroceryLists>();
            List<GroceryListItems> groceryListItems = (List<GroceryListItems>)await gridReader2.ReadAsync<GroceryListItems>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader2.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (List<UserGroceryCategories>)await gridReader2.ReadAsync<UserGroceryCategories>();
            return (true, false, groceryList, groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }

        //transaction method
        public async Task<(
            bool success,
            bool wasConflict,
            GroceryLists groceryList,
            List<GroceryListItems> listItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories)> GroceryListAddItem(GroceryListItems groceryListItem)
        {
            // Version check + bump
            string sqlBump = @"UPDATE grocery_lists SET Version = Version + 1 
                       WHERE Id = @listId AND Version = @expectedVersion";
            int bumped = await _connection.ExecuteAsync(sqlBump, new { listId = groceryListItem.ListId, expectedVersion = groceryListItem.ListVersion }, transaction: _transaction);
            if (bumped == 0)
            {
                string sql = @"
                        SELECT * FROM grocery_lists WHERE Id = @listId;
                        SELECT * FROM grocery_list_items WHERE ListId = @listId ORDER BY SortOrder, Category, GroceryItem;
                        SELECT * FROM grocery_categories ORDER BY Category;
                        SELECT DISTINCT Category, SortOrder FROM grocery_list_items WHERE ListId = @listId ORDER BY SortOrder, Category;
                        ";
                GridReader gridReader = await _connection.QueryMultipleAsync(sql, new { listId = groceryListItem.ListId }, transaction: _transaction);
                GroceryLists current = await gridReader.ReadSingleAsync<GroceryLists>();
                List<GroceryListItems> currentItems = (List<GroceryListItems>)await gridReader.ReadAsync<GroceryListItems>();
                IEnumerable<GroceryCategories> currentGroceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
                List<UserGroceryCategories> currentUserGroceryCategories = (List<UserGroceryCategories>)await gridReader.ReadAsync<UserGroceryCategories>();

                return (false, true, current, currentItems, currentGroceryCategoriesEnum, currentUserGroceryCategories);
            }
            //before insert get the sort order of the current category if no category yet set sort order to 1
            string sortOrderSql = @"SELECT SortOrder FROM grocery_list_items WHERE Category = @category AND ListId = @listId";
            int? sortOrder = await _connection.QueryFirstOrDefaultAsync<int?>(sortOrderSql, new { category = groceryListItem.Category, listId = groceryListItem.ListId }, transaction: _transaction);
            if (sortOrder == null)
            {
                sortOrder = 1;
            }
            //insert
            string insertSql = @"INSERT INTO grocery_list_items (ListId, GroceryItem, Category, Complete, SortOrder) VALUES (@listId, @groceryItem, @category, @complete, @sortOrder)";
            int rowCount = await _connection.ExecuteAsync(insertSql, new
            {
                listId = groceryListItem.ListId,
                groceryItem = groceryListItem.GroceryItem,
                category = groceryListItem.Category,
                complete = groceryListItem.Complete,
                sortOrder = sortOrder
            }, transaction: _transaction);

            string sql2 = @"
                        SELECT * FROM grocery_lists WHERE Id = @listId;
                        SELECT * FROM grocery_list_items WHERE ListId = @listId ORDER BY SortOrder, Category, GroceryItem;
                        SELECT * FROM grocery_categories ORDER BY Category;
                        SELECT DISTINCT Category, SortOrder FROM grocery_list_items WHERE ListId = @listId ORDER BY SortOrder, Category;
                        ";
            GridReader gridReader2 = await _connection.QueryMultipleAsync(sql2, new { listId = groceryListItem.ListId }, transaction: _transaction);
            GroceryLists groceryList = await gridReader2.ReadSingleAsync<GroceryLists>();
            List<GroceryListItems> groceryListItems = (List<GroceryListItems>)await gridReader2.ReadAsync<GroceryListItems>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader2.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (List<UserGroceryCategories>)await gridReader2.ReadAsync<UserGroceryCategories>();
            return (true, false, groceryList, groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }
    }
}
