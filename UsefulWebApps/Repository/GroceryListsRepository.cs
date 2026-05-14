using Dapper;
using MySqlConnector;
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

        //transaction method
        public async Task<(
            bool success, 
            bool wasConflict, 
            GroceryLists groceryList, 
            List<GroceryListItems> listItems, 
            IEnumerable<GroceryCategories> groceryCategoriesEnum, 
            List<UserGroceryCategories> userGroceryCategories)> GroceryListToggleComplete(long? id, long? listId, int expectedVersion)
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
            //toggle
            bool isComplete = await _connection.QuerySingleAsync<bool>(
                "SELECT Complete FROM grocery_list_items WHERE Id = @id", new { id }, transaction: _transaction);
            string sqlToggle = isComplete
                ? "UPDATE grocery_list_items SET Complete = False WHERE Id = @id"
                : "UPDATE grocery_list_items SET Complete = True WHERE Id = @id";
            await _connection.ExecuteAsync(sqlToggle, new { id }, transaction: _transaction);
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
           List<UserGroceryCategories> userGroceryCategories)> GroceryListSortCategories(long? listId, int newSortOrder, int expectedVersion, string category)
        {
            // Version check + bump;
            bool bumped = await _helper.TryBumpVersion(listId, expectedVersion, _transaction);
            if (!bumped)
            {
                (GroceryLists groceryList,
                List<GroceryListItems> listItems,
                IEnumerable<GroceryCategories> groceryCategoriesEnum,
                List<UserGroceryCategories> userGroceryCategories) conflictViewState = await _helper.GetListViewState(listId, _transaction);
                return (false, true, conflictViewState.groceryList, conflictViewState.listItems, conflictViewState.groceryCategoriesEnum, conflictViewState.userGroceryCategories);
            }
            string updateSql = @"UPDATE grocery_list_items SET SortOrder = @newSortOrder WHERE ListId = @listId AND Category = @category";
            await _connection.ExecuteAsync(updateSql, new { newSortOrder, listId, category }, transaction: _transaction);
            (GroceryLists groceryList,
            List<GroceryListItems> listItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories) viewState = await _helper.GetListViewState(listId, _transaction);
            return (true, false, viewState.groceryList, viewState.listItems, viewState.groceryCategoriesEnum, viewState.userGroceryCategories);
        }
    }
}
