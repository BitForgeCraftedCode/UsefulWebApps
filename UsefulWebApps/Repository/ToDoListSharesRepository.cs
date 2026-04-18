using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class ToDoListSharesRepository : Repository<ToDoListShares>, IToDoListSharesRepository
    {
        public ToDoListSharesRepository(MySqlConnection connection) : base(connection) { }
        //any ToDoListShares model specific database methods here

        public async Task<bool> ShareToDoList(long listId, string sharedWithUserId)
        {
            //IGNORE so it will not throw duplicate key error if trying to share same list with same user twice
            string sql = @"INSERT IGNORE INTO to_do_list_shares (ListId, SharedWithUserId) 
                           VALUES (@listId, @sharedWithUserId)";
            int rows = await _connection.ExecuteAsync(sql, new { listId, sharedWithUserId });
            return rows > 0;
        }

        public async Task<bool> UnshareToDoList(long listId)
        {
            string sql = "DELETE FROM to_do_list_shares WHERE ListId = @listId";
            int rows = await _connection.ExecuteAsync(sql, new { listId });
            return rows > 0;
        }
        public async Task<List<ToDoLists>> GetToDoListsSharedWithUser(string userId)
        {
            string sql = @"SELECT td.* FROM to_do_lists td
                            INNER JOIN to_do_list_shares tds ON tds.ListId = td.Id
                            WHERE tds.SharedWithUserId = @userId";

            List<ToDoLists> lists = (List<ToDoLists>)await _connection.QueryAsync<ToDoLists>(sql, new { userId });
            return lists;
        }
        // Key: ListId, Value: list of display names the list is shared with
        public async Task<Dictionary<long, List<string>>> GetSharedToMapForOwner(string ownerUserId)
        {
            // For all lists owned by this user that have been shared, return noteId -> friend display names
            // (ListId, DisplayName of the user the list is shared with)
            string sql = @"SELECT tds.ListId, COALESCE(up.DisplayName, 'Unknown') AS DisplayName
                           FROM to_do_list_shares tds
                           INNER JOIN to_do_lists td ON td.Id = tds.ListId
                           INNER JOIN user_profiles up ON up.UserId = tds.SharedWithUserId
                           WHERE td.UserId = @ownerUserId";

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
