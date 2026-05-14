using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.Helpers;
using UsefulWebApps.Repository.IRepository;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository
{
    public class GroceryListItemsRepository : Repository<GroceryListItems>, IGroceryListItemsRepository
    {
        private readonly GroceryListRepositoryHelper _helper;
        public GroceryListItemsRepository(MySqlConnection connection) : base(connection) 
        {
            _helper = new GroceryListRepositoryHelper(connection);
        }

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
            bool bumped = await _helper.TryBumpVersion(groceryListItem.ListId, groceryListItem.ListVersion, _transaction);
            // conflict
            if (!bumped) return (false, true);

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
            bool bumped = await _helper.TryBumpVersion(listId, expectedVersion, _transaction);
            if (!bumped)
            {
                (GroceryLists groceryList, 
                List<GroceryListItems> listItems, 
                IEnumerable<GroceryCategories> groceryCategoriesEnum, 
                List<UserGroceryCategories> userGroceryCategories) conflictViewState = await _helper.GetListViewState(listId, _transaction);
                return (false, true, conflictViewState.groceryList, conflictViewState.listItems, conflictViewState.groceryCategoriesEnum, conflictViewState.userGroceryCategories);
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
            (GroceryLists groceryList,
            List<GroceryListItems> listItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories) viewState = await _helper.GetListViewState(listId, _transaction);
            return (true, false, viewState.groceryList, viewState.listItems, viewState.groceryCategoriesEnum, viewState.userGroceryCategories);
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
            bool bumped = await _helper.TryBumpVersion(groceryListItem.ListId, groceryListItem.ListVersion, _transaction);
            if (!bumped)
            {
                (GroceryLists groceryList,
                List<GroceryListItems> listItems,
                IEnumerable<GroceryCategories> groceryCategoriesEnum,
                List<UserGroceryCategories> userGroceryCategories) conflictViewState = await _helper.GetListViewState(groceryListItem.ListId, _transaction);
                return (false, true, conflictViewState.groceryList, conflictViewState.listItems, conflictViewState.groceryCategoriesEnum, conflictViewState.userGroceryCategories);
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

            (GroceryLists groceryList,
            List<GroceryListItems> listItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories) viewState = await _helper.GetListViewState(groceryListItem.ListId, _transaction);
            return (true, false, viewState.groceryList, viewState.listItems, viewState.groceryCategoriesEnum, viewState.userGroceryCategories);
        }

        //transaction method
        public async Task<bool> SaveUserGroceryListTemplate(string userId, long? listId)
        {
            int rowsEffected = 0;
            string sql = "DELETE FROM grocery_list_templates WHERE UserId = @userId";
            await _connection.ExecuteAsync(sql, new { userId }, transaction: _transaction);
            string sql1 = @"INSERT INTO grocery_list_templates (UserId, GroceryItem, Category, Complete, SortOrder) 
                            SELECT @userId, GroceryItem, Category, Complete, SortOrder 
                            FROM grocery_list_items WHERE ListId = @listId";
            rowsEffected = await _connection.ExecuteAsync(sql1, new { userId, listId }, transaction: _transaction);
            return rowsEffected > 0 ? true : false;
        }

        //transaction method
        public async Task<(bool success, bool wasConflict)> UseSavedGroceryListTemplate(string userId, long? listId, int expectedVersion)
        {
            // Version check + bump
            bool bumped = await _helper.TryBumpVersion(listId, expectedVersion, _transaction);
            if (!bumped)
                return (false, true);
            
            //check if there is a saved list
            string countSql = "SELECT COUNT(Id) FROM grocery_list_templates WHERE UserId = @userId";
            int savedListRows = await _connection.QuerySingleAsync<int>(countSql, new { userId }, transaction: _transaction);
            if (savedListRows == 0)
                return (false, false);
            
            //delete current list
            string deleteSql = "DELETE FROM grocery_list_items WHERE ListId = @listId";
            await _connection.ExecuteAsync(deleteSql, new { listId }, transaction: _transaction);
            //insert saved list as current
            string insertSql = @"INSERT INTO grocery_list_items (ListId, GroceryItem, Category, Complete, SortOrder) 
                            SELECT @listId, GroceryItem, Category, Complete, SortOrder 
                            FROM grocery_list_templates WHERE UserId = @userId";
            int rowsAffected = await _connection.ExecuteAsync(insertSql, new { listId, userId }, transaction: _transaction);
            
            return (rowsAffected > 0, false);
        }
    }
}
