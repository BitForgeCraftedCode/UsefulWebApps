using Dapper;
using MySqlConnector;

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
    }
}
