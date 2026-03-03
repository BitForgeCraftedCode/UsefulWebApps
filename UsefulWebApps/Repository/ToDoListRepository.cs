using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;

namespace UsefulWebApps.Repository
{
    public class ToDoListRepository : Repository<ToDoList>, IToDoListRepository
    {
        public ToDoListRepository(MySqlConnection connection) : base(connection) { }

        //any ToDoList model specific database methods here

        //transaction method
        public async Task<List<ToDoList>> ToDoListToggleComplete(int? id, string userId, string listTitle)
        {
           
            //toggle complete
            string sql = "SELECT Complete FROM to_do_list WHERE Id = @id";
            bool isComplete = await _connection.QuerySingleAsync<bool>(sql, new { id }, transaction: _transaction);
            string sql2 = String.Empty;
            if (isComplete)
            {
                sql2 = "UPDATE to_do_list SET Complete = False WHERE Id = @id";
            }
            else
            {
                sql2 = "UPDATE to_do_list SET Complete = True WHERE Id = @id";
            }
            await _connection.ExecuteAsync(sql2, new { id }, transaction: _transaction);
            //get all list items for userId
            string sql3 = "SELECT * FROM to_do_list WHERE UserId = @userId AND ListTitle = @listTitle";
            List<ToDoList> allDbRows = (List<ToDoList>)await _connection.QueryAsync<ToDoList>(sql3, new { userId, listTitle }, transaction: _transaction);
            return allDbRows;
        }

        //transaction method
        public async Task<List<ToDoList>> ToDoListAdd(ToDoList toDoList)
        {
            int rowsEffected = 0;

            string sql = "INSERT INTO to_do_list (ToDoItem, Complete, UserId, ListTitle) VALUES (@toDoItem, @complete, @userId, @listTitle)";
            rowsEffected = await _connection.ExecuteAsync(sql, new 
            { 
                toDoItem = toDoList.ToDoItem,
                complete = toDoList.Complete,
                userId = toDoList.UserId,
                listTitle = toDoList.ListTitle,
            }, transaction: _transaction);
            //get all list items for userId
            string sql2 = "SELECT * FROM to_do_list WHERE UserId = @userId AND ListTitle = @listTitle";
            List<ToDoList> allDbRows = (List<ToDoList>)await _connection.QueryAsync<ToDoList>(sql2, new { userId = toDoList.UserId, listTitle = toDoList.ListTitle }, transaction: _transaction);
            return allDbRows;
        }

        public async Task<List<string>> GetMyToDoLists(string userId)
        {
            string sql = @"SELECT DISTINCT ListTitle FROM to_do_list WHERE UserId = @userId";
            List<string> myToDoLists = (List<string>)await _connection.QueryAsync<string>(sql, new { userId });
            return myToDoLists;
        }

        public async Task<List<ToDoList>> GetAllItemsInList(string userId, string list)
        {
            string sql = @"SELECT * FROM to_do_list WHERE UserId = @userId AND ListTitle = @list";
            List<ToDoList> toDoList = (List<ToDoList>)await _connection.QueryAsync<ToDoList>(sql, new { userId, list });
            return toDoList;
        }

        public async Task<bool> DeleteAllItemsInList(string userId, string list)
        {
            int rowsEffected = 0;
            string sql = @"DELETE FROM to_do_list WHERE UserID = @userId AND ListTitle = @list";
            rowsEffected = await _connection.ExecuteAsync(sql, new { userId, list });
            return rowsEffected > 0 ? true : false;
        }
    }
}
