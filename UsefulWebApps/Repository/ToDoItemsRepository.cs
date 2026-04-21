using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class ToDoItemsRepository : Repository<ToDoItems>, IToDoItemsRepository
    {
        public ToDoItemsRepository(MySqlConnection connection) : base(connection) { }
        //any ToDoItems model specific database methods here

        //transaction method -- Concurrent
        public async Task<(bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListAddItem(ToDoItems toDoItem)
        {
            // Version check + bump on parent list
            string sqlBump = @"UPDATE to_do_lists SET Version = Version + 1 
                       WHERE Id = @listId AND Version = @listVersion";
            int bumped = await _connection.ExecuteAsync(sqlBump,
                new { listId = toDoItem.ListId, listVersion = toDoItem.ListVersion }, transaction: _transaction);
            if (bumped == 0)
            {
                ToDoLists current = await _connection.QuerySingleAsync<ToDoLists>(
                    "SELECT * FROM to_do_lists WHERE Id = @listId", new { listId = toDoItem.ListId }, transaction: _transaction);
                List<ToDoItems> currentItems = (List<ToDoItems>)await _connection.QueryAsync<ToDoItems>(
                    "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder", new { listId = toDoItem.ListId }, transaction: _transaction);
                return (false, true, current, currentItems);
            }
            string sql = @"INSERT INTO to_do_items (ListId, ToDoItem, Complete, SortOrder) 
                   VALUES (@listId, @toDoItem, @complete, @sortOrder)";
            await _connection.ExecuteAsync(sql, new
            {
                listId = toDoItem.ListId,
                toDoItem = toDoItem.ToDoItem,
                complete = toDoItem.Complete,
                sortOrder = toDoItem.SortOrder,
            }, transaction: _transaction);
            ToDoLists toDoList = await _connection.QuerySingleAsync<ToDoLists>(
                "SELECT * FROM to_do_lists WHERE Id = @listId", new { listId = toDoItem.ListId }, transaction: _transaction);
            List<ToDoItems> listItems = (List<ToDoItems>)await _connection.QueryAsync<ToDoItems>(
                "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder", new { listId = toDoItem.ListId }, transaction: _transaction);
            return (true, false, toDoList, listItems);
        }
    }
}
