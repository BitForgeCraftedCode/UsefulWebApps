using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;

namespace UsefulWebApps.Repository
{
    public class ToDoListRepository : Repository<ToDoList>, IToDoListRepository
    {
        private readonly MySqlConnection _connection;
        public ToDoListRepository(MySqlConnection db) : base(db)
        {
            _connection = db;
        }

        //any ToDoList model specific database methods here
        public async Task ToDoListToggleComplete(int? id)
        {
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            string sql = "SELECT Complete FROM to_do_list WHERE Id = @id";
            bool isComplete = await _connection.QuerySingleAsync<bool>(sql, new { id }, transaction: txn);
            string sql2 = String.Empty;
            if (isComplete)
            {
                sql2 = "UPDATE to_do_list SET Complete = False WHERE Id = @id";
            }
            else
            {
                sql2 = "UPDATE to_do_list SET Complete = True WHERE Id = @id";
            }
            await _connection.ExecuteAsync(sql2, new { id }, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
        }
    }
}
