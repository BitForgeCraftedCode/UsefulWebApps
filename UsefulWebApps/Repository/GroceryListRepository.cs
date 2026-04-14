using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository
{
    public class GroceryListRepository : Repository<GroceryList>, IGroceryListRepository
    {
        public GroceryListRepository(MySqlConnection connection) : base(connection) { }
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
            return (groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }

        public async Task<(GroceryList groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemAndCategoriesAtId(long? id)
        {
            string query = @"
                SELECT * FROM grocery_list WHERE Id = @id;
                SELECT * FROM grocery_categories ORDER BY Category ASC;
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(query, new { id });
            GroceryList groceryListItem = await gridReader.ReadFirstAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            return (groceryListItem, groceryCategoriesEnum);
        }

        //transaction method
        public async Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories)> GroceryListToggleComplete(long? id, string userId)
        {
            string sql = "SELECT Complete FROM grocery_list WHERE Id = @id";
            bool isComplete = await _connection.QuerySingleAsync<bool>(sql, new { id }, transaction: _transaction);
            string sql2 = String.Empty;
            if (isComplete)
            {
                sql2 = "UPDATE grocery_list SET Complete = False WHERE Id = @id";
            }
            else
            {
                sql2 = "UPDATE grocery_list SET Complete = True WHERE Id = @id";
            }
            await _connection.ExecuteAsync(sql2, new { id }, transaction: _transaction);
            string sql3 = $@"
                SELECT * FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC, GroceryItem ASC;
                SELECT * FROM grocery_categories ORDER BY Category ASC;
                SELECT DISTINCT Category, SortOrder FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC; 
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sql3, new { Parameter = userId }, transaction: _transaction);
            List<GroceryList> groceryListItems = (List<GroceryList>)await gridReader.ReadAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (List<UserGroceryCategories>)await gridReader.ReadAsync<UserGroceryCategories>();
            return (groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }

        //transaction method
        public async Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories)> GroceryListSortCategories(int sortOrder, string category, string userId)
        {
            string sql = "UPDATE grocery_list SET SortOrder = @sortOrder WHERE Category = @category AND UserId = @userId";
            await _connection.ExecuteAsync(sql, new { sortOrder, category, userId }, transaction: _transaction);
            string sql2 = $@"
                SELECT * FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC, GroceryItem ASC;
                SELECT * FROM grocery_categories ORDER BY Category ASC;
                SELECT DISTINCT Category, SortOrder FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC; 
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sql2, new { Parameter = userId }, transaction: _transaction);
            List<GroceryList> groceryListItems = (List<GroceryList>)await gridReader.ReadAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (List<UserGroceryCategories>)await gridReader.ReadAsync<UserGroceryCategories>();
            return (groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }

        //transaction method
        public async Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories)> GroceryListAdd(GroceryList groceryList)
        {
            //before insert get the sort order of the current category if no category yet set sort order to 1
            string sql = "SELECT SortOrder FROM grocery_list WHERE Category = @category AND UserId = @userId";
            int? sortOrder = await _connection.QueryFirstOrDefaultAsync<int?>(sql, new { category = groceryList.Category, userId = groceryList.UserId}, transaction: _transaction);
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
            }, transaction: _transaction);
            string sql2 = $@"
                SELECT * FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC, GroceryItem ASC;
                SELECT * FROM grocery_categories ORDER BY Category ASC;
                SELECT DISTINCT Category, SortOrder FROM grocery_list WHERE UserId = @Parameter ORDER BY SortOrder ASC, Category ASC; 
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sql2, new { Parameter = groceryList.UserId }, transaction: _transaction);
            List<GroceryList> groceryListItems = (List<GroceryList>)await gridReader.ReadAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            List<UserGroceryCategories> userGroceryCategories = (List<UserGroceryCategories>)await gridReader.ReadAsync<UserGroceryCategories>();
            return (groceryListItems, groceryCategoriesEnum, userGroceryCategories);
        }

        //transaction method
        public async Task<bool> GroceryListUpdate(GroceryList groceryList)
        {
            int rowsEffected = 0;
            //before update get the sort order of the category if no category yet set sort order to 1
            string sql = "SELECT SortOrder FROM grocery_list WHERE Category = @category AND UserId = @userId";
            int? sortOrder = await _connection.QueryFirstOrDefaultAsync<int?>(sql, new { category = groceryList.Category, userId = groceryList.UserId }, transaction: _transaction);
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
            }, transaction: _transaction);
            return rowsEffected > 0 ? true : false;
        }

        //transaction method
        public async Task<bool> SaveUserGroceryList(string userId)
        {
            int rowsEffected = 0;
            string sql = "DELETE FROM grocery_list_usersaved WHERE UserId = @userId";
            await _connection.ExecuteAsync(sql, new { userId }, transaction: _transaction);
            string sql1 = "INSERT INTO grocery_list_usersaved (GroceryItem, Category, Complete, UserId, SortOrder) SELECT GroceryItem, Category, Complete, UserId, SortOrder FROM grocery_list WHERE UserId = @userId";
            rowsEffected = await _connection.ExecuteAsync(sql1, new { userId } , transaction: _transaction);
            return rowsEffected > 0 ? true : false;
        }

        //transaction method
        public async Task<bool> UseSavedGroceryList(string userId)
        {
            int rowsEffected = 0;
            //check there is a saved list
            string sql1 = "SELECT COUNT(Id) FROM grocery_list_usersaved WHERE UserId = @userId";
            int savedListRows =  await _connection.QuerySingleAsync<int>(sql1, new { userId }, transaction: _transaction);
            if (savedListRows == 0) 
            {
                //false rollback
                return rowsEffected > 0 ? true : false;
            }
            //delete current list
            string sql2 = "DELETE FROM grocery_list WHERE UserId = @userId";
            await _connection.ExecuteAsync(sql2, new { userId }, transaction: _transaction);
            //insert saved list as current
            string sql3 = "INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId, SortOrder) SELECT GroceryItem, Category, Complete, UserId, SortOrder FROM grocery_list_usersaved WHERE UserId = @userId";
            rowsEffected = await _connection.ExecuteAsync(sql3, new { userId }, transaction: _transaction);
            return rowsEffected > 0 ? true : false;
        }

        //transaction method
        public async Task<bool> ShareGroceryList(string userId, string friendUserId)
        {
            int rowsEffected = 0;
            //check if friend has a saved list
            string sql1 = "SELECT COUNT(Id) FROM grocery_list WHERE UserId = @friendUserId";
            int savedListRows = await _connection.QuerySingleAsync<int>(sql1, new { friendUserId }, transaction: _transaction);
            if (savedListRows == 0)
            {
                //false rollback
                return rowsEffected > 0 ? true : false;
            }
            //add user's id to ShareUserId col of friend's list
            string sql = "UPDATE grocery_list SET ShareUserId = @userId WHERE UserId = @friendUserId";
            await _connection.ExecuteAsync(sql, new { userId, friendUserId }, transaction: _transaction);
            //delete user's grocery list
            string sql2 = "DELETE FROM grocery_list WHERE UserId = @userId";
            await _connection.ExecuteAsync(sql2, new { userId }, transaction: _transaction);  
            //insert friends list into users -- NOTE UserId here is ShareUserId from friend list
            string sql3 = "INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId, SortOrder) SELECT GroceryItem, Category, Complete, ShareUserId, SortOrder FROM grocery_list WHERE UserId = @friendUserId";
            rowsEffected = await _connection.ExecuteAsync(sql3, new { friendUserId }, transaction: _transaction);
            //commit txn and close connection
            return rowsEffected > 0 ? true : false;
        }
    }
}
