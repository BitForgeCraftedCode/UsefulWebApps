using Dapper;
using MySqlConnector;
using UsefulWebApps.DTO.ListBuddy;
using UsefulWebApps.Models.ListBuddy;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository.Helpers
{
    public class ToDoListRepositoryHelper
    {
        private readonly MySqlConnection _connection;

        public ToDoListRepositoryHelper(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> TryBumpVersion(
            long? listId, 
            int expectedVersion, 
            MySqlTransaction? transaction)
        {
            string sql = @"
                UPDATE to_do_lists
                SET Version = Version + 1
                WHERE Id = @listId 
                AND Version = @expectedVersion;
            ";

            int rows = await _connection.ExecuteAsync(
                sql,
                new { listId, expectedVersion },
                transaction: transaction);

            return rows > 0;
        }

        public async Task<ToDoListViewState> GetListViewState(long? listId, MySqlTransaction? transaction)
        {
            string sql = @"
                SELECT * FROM to_do_lists WHERE Id = @listId;

                SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder, ToDoItem;
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sql, new { listId }, transaction: transaction);

            ToDoLists current = await gridReader.ReadSingleAsync<ToDoLists>();
            List<ToDoItems> currentItems = (await gridReader.ReadAsync<ToDoItems>()).ToList();

            return new ToDoListViewState 
            { 
                ToDoList = current,
                ListItems = currentItems
            };
        }
    }
}
