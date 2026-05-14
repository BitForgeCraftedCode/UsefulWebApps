using MySqlConnector;
using Dapper;

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
    }
}
