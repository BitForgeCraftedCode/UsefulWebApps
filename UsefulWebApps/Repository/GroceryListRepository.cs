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

        //return multiple types with a tuple https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples 
        public async Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories)> GetGroceryListItemsAndCategories(string column, string value)
        {
            string query = $@"
                    SELECT * FROM grocery_list WHERE {column} = @Parameter ORDER BY SortOrder ASC, Category ASC, GroceryItem ASC;
                    SELECT * FROM grocery_categories ORDER BY Category ASC;
                    SELECT DISTINCT Category, SortOrder FROM grocery_list WHERE {column} = @Parameter ORDER BY SortOrder ASC, Category ASC; 
                ";
            GridReader gridReader = await _connection.QueryMultipleAsync(query, new { Parameter = value});
            List<GroceryList> groceryListItems = (List<GroceryList>)await gridReader.ReadAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (List<UserGroceryCategories>)await gridReader.ReadAsync<UserGroceryCategories>();
            await _connection.CloseAsync();
            return (groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }

        public async Task<(GroceryList groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemAndCategoriesAtId(int? id)
        {
            string query = @"
                SELECT * FROM grocery_list WHERE Id = @id;
                SELECT * FROM grocery_categories ORDER BY Category ASC;
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(query, new { id });
            GroceryList groceryListItem = await gridReader.ReadFirstAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            await _connection.CloseAsync();
            return (groceryListItem, groceryCategoriesEnum);
        }

        public async Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories)> GroceryListToggleComplete(int? id, string userId)
        {
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            string sql = "SELECT Complete FROM grocery_list WHERE Id = @id";
            bool isComplete = await _connection.QuerySingleAsync<bool>(sql, new { id }, transaction: txn);
            string sql2 = String.Empty;
            if (isComplete)
            {
                sql2 = "UPDATE grocery_list SET Complete = False WHERE Id = @id";
            }
            else
            {
                sql2 = "UPDATE grocery_list SET Complete = True WHERE Id = @id";
            }
            await _connection.ExecuteAsync(sql2, new { id }, transaction: txn);
            string sql3 = $@"
                SELECT * FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC, GroceryItem ASC;
                SELECT * FROM grocery_categories ORDER BY Category ASC;
                SELECT DISTINCT Category, SortOrder FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC; 
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sql3, new { Parameter = userId }, transaction: txn);
            List<GroceryList> groceryListItems = (List<GroceryList>)await gridReader.ReadAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (List<UserGroceryCategories>)await gridReader.ReadAsync<UserGroceryCategories>();
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return (groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }

        public async Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories)> GroceryListSortCategories(int sortOrder, string category, string userId)
        {
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            string sql = "UPDATE grocery_list SET SortOrder = @sortOrder WHERE Category = @category AND UserId = @userId";
            await _connection.ExecuteAsync(sql, new { sortOrder, category, userId }, transaction: txn);
            string sql2 = $@"
                SELECT * FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC, GroceryItem ASC;
                SELECT * FROM grocery_categories ORDER BY Category ASC;
                SELECT DISTINCT Category, SortOrder FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC; 
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sql2, new { Parameter = userId }, transaction: txn);
            List<GroceryList> groceryListItems = (List<GroceryList>)await gridReader.ReadAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (List<UserGroceryCategories>)await gridReader.ReadAsync<UserGroceryCategories>();
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return (groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }

        public async Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories)> GroceryListAdd(GroceryList groceryList)
        {
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            //before insert get the sort order of the current category if no category yet set sort order to 1
            string sql = "SELECT SortOrder FROM grocery_list WHERE Category = @category AND UserId = @userId";
            int? sortOrder = await _connection.QueryFirstOrDefaultAsync<int?>(sql, new { category = groceryList.Category, userId = groceryList.UserId}, transaction: txn);
            if (sortOrder == null) 
            {
                sortOrder = 1; 
            }
            string sql1 = "INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId, SortOrder) VALUES (@groceryItem, @category, @complete, @userId, @sortOrder)";
            await _connection.ExecuteAsync(sql1, new
            {
                groceryItem = groceryList.GroceryItem,
                category = groceryList.Category,
                complete = groceryList.Complete,
                userId = groceryList.UserId,
                sortOrder = sortOrder,
            }, transaction: txn);
            string sql2 = $@"
                SELECT * FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC, GroceryItem ASC;
                SELECT * FROM grocery_categories ORDER BY Category ASC;
                SELECT DISTINCT Category, SortOrder FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC; 
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sql2, new { Parameter = groceryList.UserId }, transaction: txn);
            List<GroceryList> groceryListItems = (List<GroceryList>)await gridReader.ReadAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (List<UserGroceryCategories>)await gridReader.ReadAsync<UserGroceryCategories>();
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return (groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }

        public async Task<bool> GroceryListUpdate(GroceryList groceryList)
        {
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            int rowsEffected = 0;
            //before update get the sort order of the category if no category yet set sort order to 1
            string sql = "SELECT SortOrder FROM grocery_list WHERE Category = @category AND UserId = @userId";
            int? sortOrder = await _connection.QueryFirstOrDefaultAsync<int?>(sql, new { category = groceryList.Category, userId = groceryList.UserId }, transaction: txn);
            if (sortOrder == null)
            {
                sortOrder = 1;
            }
            string sql1 = "UPDATE grocery_list SET GroceryItem = @groceryItem, Category = @category, Complete = @complete, UserId = @userId, SortOrder = @sortOrder WHERE Id = @id";
            rowsEffected = await _connection.ExecuteAsync(sql1, new 
            { 
                groceryItem = groceryList.GroceryItem,
                category = groceryList.Category,
                complete = groceryList.Complete,
                userId = groceryList.UserId,
                sortOrder = sortOrder,
                id = groceryList.Id
            }, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return rowsEffected > 0 ? true : false;
        }

        public async Task<bool> SaveUserGroceryList(string userId)
        {
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            int rowsEffected = 0;
            string sql = "DELETE FROM grocery_list_usersaved WHERE UserId = @userId";
            await _connection.ExecuteAsync(sql, new { userId }, transaction: txn);
            string sql1 = "INSERT INTO grocery_list_usersaved (GroceryItem, Category, Complete, UserId, SortOrder) SELECT GroceryItem, Category, Complete, UserId, SortOrder FROM grocery_list WHERE UserId = @userId";
            rowsEffected = await _connection.ExecuteAsync(sql1, new { userId }, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return rowsEffected > 0 ? true : false;
        }
    }
}
