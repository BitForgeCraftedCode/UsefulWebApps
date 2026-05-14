using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository.Helpers
{
    public class GroceryListRepositoryHelper
    {
        private readonly MySqlConnection _connection;

        public GroceryListRepositoryHelper(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> TryBumpVersion(
            long? listId,
            int expectedVersion,
            MySqlTransaction? transaction)
        {
            string sql = @"
            UPDATE grocery_lists
            SET Version = Version + 1
            WHERE Id = @listId
            AND Version = @expectedVersion";

            int rows = await _connection.ExecuteAsync(
                sql,
                new { listId, expectedVersion },
                transaction: transaction);

            return rows > 0;
        }

        public async Task<(
            GroceryLists groceryList,
            List<GroceryListItems> listItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories)> GetListViewState(long? listId, MySqlTransaction? transaction)
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
            GridReader gridReader = await _connection.QueryMultipleAsync(sql, new { listId }, transaction: transaction);

            GroceryLists current = await gridReader.ReadSingleAsync<GroceryLists>();
            List<GroceryListItems> currentItems = (await gridReader.ReadAsync<GroceryListItems>()).ToList();
            IEnumerable<GroceryCategories> currentGroceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> currentUserGroceryCategories = (await gridReader.ReadAsync<UserGroceryCategories>()).ToList();

            return (current, currentItems, currentGroceryCategoriesEnum, currentUserGroceryCategories);
        }
    }
}
