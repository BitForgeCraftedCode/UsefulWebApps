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
        public async Task<List<ToDoList>> ToDoListToggleComplete(int? id, string userId)
        {
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            //toggle complete
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
            //get all list items for userId
            string sql3 = "SELECT * FROM to_do_list WHERE UserId = @userId";
            List<ToDoList> allDbRows = (List<ToDoList>)await _connection.QueryAsync<ToDoList>(sql3, new { userId }, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return allDbRows;
        }

        public async Task<List<ToDoList>> ToDoListAdd(ToDoList toDoList)
        {
            int rowsEffected = 0;
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            string sql = "INSERT INTO to_do_list (ToDoItem, Complete, UserId) VALUES (@toDoItem, @complete, @userId)";
            rowsEffected = await _connection.ExecuteAsync(sql, new 
            { 
                toDoItem = toDoList.ToDoItem,
                complete = toDoList.Complete,
                userId = toDoList.UserId
            }, transaction: txn);
            //get all list items for userId
            string sql2 = "SELECT * FROM to_do_list WHERE UserId = @userId";
            List<ToDoList> allDbRows = (List<ToDoList>)await _connection.QueryAsync<ToDoList>(sql2, new { userId = toDoList.UserId }, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return allDbRows;
        }
    }
}
