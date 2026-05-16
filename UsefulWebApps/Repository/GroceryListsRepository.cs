using Dapper;
using MySqlConnector;
using UsefulWebApps.DTO.ListBuddy;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.Helpers;
using UsefulWebApps.Repository.IRepository;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository
{
    public class GroceryListsRepository : Repository<GroceryLists>, IGroceryListsRepository
    {
        private readonly GroceryListRepositoryHelper _helper;
        public GroceryListsRepository(MySqlConnection connection) : base(connection) 
        {
            _helper = new GroceryListRepositoryHelper(connection);
        }
        //any GroceryLists model specific database methods here

        public async Task<IEnumerable<GroceryCategories>> GetGroceryCategoriesEnum()
        {
            string sql = "SELECT * FROM grocery_categories ORDER BY Category";
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await _connection.QueryAsync<GroceryCategories>(sql);
            return groceryCategoriesEnum;
        }

        public async Task<GroceryListViewState> GetAllItemsAndCategoriesInList(long? listId)
        {
            string sql = @"
                SELECT * FROM grocery_lists WHERE Id = @listId;

                SELECT * FROM grocery_list_items 
                WHERE ListId = @listId 
                ORDER BY SortOrder, Category, GroceryItem;

                SELECT * FROM grocery_categories 
                ORDER BY Category;

                SELECT DISTINCT Category, SortOrder 
                FROM grocery_list_items
                WHERE ListId = @listId 
                ORDER BY SortOrder, Category;
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sql, new { listId });

            GroceryLists groceryList = await gridReader.ReadSingleAsync<GroceryLists>();
            List<GroceryListItems> groceryListItems = (await gridReader.ReadAsync<GroceryListItems>()).ToList();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (await gridReader.ReadAsync<UserGroceryCategories>()).ToList();

            return new GroceryListViewState
            {
                GroceryList = groceryList,
                ListItems = groceryListItems,
                GroceryCategoriesEnum = groceryCategoriesEnum,
                UserGroceryCategories = userGroceryCategories
            };
        }

        //return multiple types with a tuple https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples 
        //transaction method
        public async Task<(bool success, bool wasConflict, GroceryListViewState viewState)> GroceryListToggleComplete(long? id, long? listId, int expectedVersion)
        {
            // Version check + bump
            bool bumped = await _helper.TryBumpVersion(listId, expectedVersion, _transaction);
            if (!bumped)
            {
                GroceryListViewState conflictViewState = await _helper.GetListViewState(listId, _transaction);
                return (false, true, conflictViewState);
            }
            //toggle
            bool isComplete = await _connection.QuerySingleAsync<bool>(
                "SELECT Complete FROM grocery_list_items WHERE Id = @id", new { id }, transaction: _transaction);
            string sqlToggle = isComplete
                ? "UPDATE grocery_list_items SET Complete = False WHERE Id = @id"
                : "UPDATE grocery_list_items SET Complete = True WHERE Id = @id";
            await _connection.ExecuteAsync(sqlToggle, new { id }, transaction: _transaction);
            GroceryListViewState viewState = await _helper.GetListViewState(listId, _transaction);
            return (true, false, viewState);
        }

        //transaction method
        public async Task<(bool success, bool wasConflict, GroceryListViewState viewState)> GroceryListSortCategories(long? listId, int newSortOrder, int expectedVersion, string category)
        {
            // Version check + bump;
            bool bumped = await _helper.TryBumpVersion(listId, expectedVersion, _transaction);
            if (!bumped)
            {
                GroceryListViewState conflictViewState = await _helper.GetListViewState(listId, _transaction);
                return (false, true, conflictViewState);
            }
            string updateSql = @"UPDATE grocery_list_items SET SortOrder = @newSortOrder WHERE ListId = @listId AND Category = @category";
            await _connection.ExecuteAsync(updateSql, new { newSortOrder, listId, category }, transaction: _transaction);
            GroceryListViewState viewState = await _helper.GetListViewState(listId, _transaction);
            return (true, false, viewState);
        }
    }
}
