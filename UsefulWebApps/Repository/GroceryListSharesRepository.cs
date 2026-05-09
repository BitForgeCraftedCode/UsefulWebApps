using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;

namespace UsefulWebApps.Repository
{
    public class GroceryListSharesRepository : Repository<GroceryListShares>, IGroceryListSharesRepository
    {
        public GroceryListSharesRepository(MySqlConnection connection) : base(connection) { }
        //any GroceryListShares model specific database methods here

        public async Task<bool> ShareGroceryList(long listId, string sharedWithUserId)
        {
            //IGNORE so it will not throw duplicate key error if trying to share same list with same user twice
            string sql = @"INSERT IGNORE INTO grocery_list_shares (ListId, SharedWithUserId) 
                           VALUES (@listId, @sharedWithUserId)";
            int rows = await _connection.ExecuteAsync(sql, new { listId, sharedWithUserId });
            return rows > 0;
        }
        public async Task<List<GroceryLists>> GetGroceryListsSharedWithUser(string userId)
        {
            string sql = @"SELECT gl.* FROM grocery_lists gl
                            INNER JOIN grocery_list_shares gls ON gls.ListId = gl.Id
                            WHERE gls.SharedWithUserId = @userId";

            List<GroceryLists> lists = (List<GroceryLists>)await _connection.QueryAsync<GroceryLists>(sql, new { userId });
            return lists;
        }

        // Key: ListId, Value: list of display names the list is shared with
        public async Task<Dictionary<long, List<string>>> GetSharedToMapForOwner(string ownerUserId)
        {
            // For all lists owned by this user that have been shared, return listId -> friend display names
            // (ListId, DisplayName of the user the list is shared with)
            string sql = @"SELECT gls.ListId, COALESCE(up.DisplayName, 'Unknown') AS DisplayName
                           FROM grocery_list_shares gls
                           INNER JOIN grocery_lists gl ON gl.Id = gls.ListId
                           INNER JOIN user_profiles up ON up.UserId = gls.SharedWithUserId
                           WHERE gl.UserId = @ownerUserId";

            var rows = await _connection.QueryAsync<(long ListId, string DisplayName)>(sql, new { ownerUserId });

            Dictionary<long, List<string>> map = new();
            foreach (var row in rows)
            {
                //if map does not contain key create it
                if (!map.ContainsKey(row.ListId))
                    map[row.ListId] = new List<string>();
                //add display name to the correct key
                map[row.ListId].Add(row.DisplayName);
            }
            return map;
        }
    }
}
