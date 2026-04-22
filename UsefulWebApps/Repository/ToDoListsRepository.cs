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
        public async Task<(bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListToggleComplete(long id, long listId, int expectedVersion)
        {
            // Version check + bump
            string sqlBump = @"UPDATE to_do_lists SET Version = Version + 1 
                       WHERE Id = @listId AND Version = @expectedVersion";
            int bumped = await _connection.ExecuteAsync(sqlBump, new { listId, expectedVersion }, transaction: _transaction);
            if (bumped == 0)
            {
                ToDoLists current = await _connection.QuerySingleAsync<ToDoLists>(
                    "SELECT * FROM to_do_lists WHERE Id = @listId", new { listId }, transaction: _transaction);
                List<ToDoItems> currentItems = (List<ToDoItems>)await _connection.QueryAsync<ToDoItems>(
                    "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder", new { listId }, transaction: _transaction);
                return (false, true, current, currentItems);
            }
            // toggle
            bool isComplete = await _connection.QuerySingleAsync<bool>(
                "SELECT Complete FROM to_do_items WHERE Id = @id", new { id }, transaction: _transaction);
            string sqlToggle = isComplete
                ? "UPDATE to_do_items SET Complete = False WHERE Id = @id"
                : "UPDATE to_do_items SET Complete = True WHERE Id = @id";
            await _connection.ExecuteAsync(sqlToggle, new { id }, transaction: _transaction);
            ToDoLists toDoList = await _connection.QuerySingleAsync<ToDoLists>(
                "SELECT * FROM to_do_lists WHERE Id = @listId", new { listId }, transaction: _transaction);
            List<ToDoItems> listItems = (List<ToDoItems>)await _connection.QueryAsync<ToDoItems>(
                "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder", new { listId }, transaction: _transaction);
            return (true, false, toDoList, listItems);
        }

        //transaction method
        public async Task<(bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListSortItem(long id, long listId, int sortOrder, int expectedVersion)
        {
            // Version check + bump
            string sqlBump = @"UPDATE to_do_lists SET Version = Version + 1 
                       WHERE Id = @listId AND Version = @expectedVersion";
            int bumped = await _connection.ExecuteAsync(sqlBump, new { listId, expectedVersion }, transaction: _transaction);
            if (bumped == 0)
            {
                ToDoLists current = await _connection.QuerySingleAsync<ToDoLists>(
                    "SELECT * FROM to_do_lists WHERE Id = @listId", new { listId }, transaction: _transaction);
                List<ToDoItems> currentItems = (List<ToDoItems>)await _connection.QueryAsync<ToDoItems>(
                    "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder", new { listId }, transaction: _transaction);
                return (false, true, current, currentItems);
            }
            string sql = @"UPDATE to_do_items SET SortOrder = @sortOrder WHERE Id = @id";
            await _connection.ExecuteAsync(sql, new { sortOrder, id }, transaction: _transaction);
            ToDoLists toDoList = await _connection.QuerySingleAsync<ToDoLists>(
                "SELECT * FROM to_do_lists WHERE Id = @listId", new { listId }, transaction: _transaction);
            List<ToDoItems> listItems = (List<ToDoItems>)await _connection.QueryAsync<ToDoItems>(
                "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder", new { listId }, transaction: _transaction);
            return (true, false, toDoList, listItems);
        }
    }
}
