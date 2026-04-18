using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class ToDoListsRepository : Repository<ToDoLists>, IToDoListsRepository
    {
        public ToDoListsRepository(MySqlConnection connection) : base(connection) { }

        //any ToDoLists model specific database methods here
        public async Task<List<ToDoItems>> GetAllItemsInList(long? listId)
        {
            string sql = "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder";
            List<ToDoItems> items = (List<ToDoItems>)await _connection.QueryAsync<ToDoItems>(sql, new { listId });
            return items;
        }

        //transaction method
        public async Task<(ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListToggleComplete(long id, long listId)
        {
            string sql = "SELECT * FROM to_do_lists WHERE Id = @listId";
            ToDoLists toDoList = await _connection.QuerySingleAsync<ToDoLists>(sql, new { listId }, transaction: _transaction);
            //toggle complete
            string sql1 = "SELECT Complete FROM to_do_items WHERE Id = @id";
            bool isComplete = await _connection.QuerySingleAsync<bool>(sql1, new { id }, transaction: _transaction);
            string sql2 = String.Empty;
            if (isComplete)
            {
                sql2 = "UPDATE to_do_items SET Complete = False WHERE Id = @id";
            }
            else
            {
                sql2 = "UPDATE to_do_items SET Complete = True WHERE Id = @id";
            }
            await _connection.ExecuteAsync(sql2, new { id }, transaction: _transaction);
            //get all list tiems for listId
            string sql3 = "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder";
            List<ToDoItems> listItems = (List<ToDoItems>)await _connection.QueryAsync<ToDoItems>(sql3, new { listId }, transaction: _transaction);
            return (toDoList, listItems);

        }
    }
}
