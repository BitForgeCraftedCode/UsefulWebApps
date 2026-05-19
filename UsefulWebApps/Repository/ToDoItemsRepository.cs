using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.Helpers;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class ToDoItemsRepository : Repository<ToDoItems>, IToDoItemsRepository
    {
        private readonly ToDoListRepositoryHelper _helper;
        public ToDoItemsRepository(MySqlConnection connection) : base(connection) 
        { 
            _helper = new ToDoListRepositoryHelper(connection);
        }
        //any ToDoItems model specific database methods here

        //transaction method -- Concurrent
        public async Task<(bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListAddItem(ToDoItems toDoItem)
        {
            // Version check + bump on parent list
            bool bumped = await _helper.TryBumpVersion(toDoItem.ListId, toDoItem.ListVersion, _transaction);
            if (!bumped)
            {
                ToDoLists current = await _connection.QuerySingleAsync<ToDoLists>(
                    "SELECT * FROM to_do_lists WHERE Id = @listId", new { listId = toDoItem.ListId }, transaction: _transaction);
                List<ToDoItems> currentItems = (await _connection.QueryAsync<ToDoItems>(
                    "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder, ToDoItem", new { listId = toDoItem.ListId }, transaction: _transaction)).ToList();
                return (false, true, current, currentItems);
            }
            //get row count 
            //set SortOrder to Count of existing items so new items always land at the bottom cleanly
            string countSql = @"SELECT COUNT(*) FROM to_do_items WHERE ListId = @listId";
            int rowCount = await _connection.QuerySingleAsync<int>(countSql, new { listId = toDoItem.ListId }, transaction: _transaction);

            string sql = @"INSERT INTO to_do_items (ListId, ToDoItem, Complete, SortOrder) 
                   VALUES (@listId, @toDoItem, @complete, @sortOrder)";
            await _connection.ExecuteAsync(sql, new
            {
                listId = toDoItem.ListId,
                toDoItem = toDoItem.ToDoItem,
                complete = toDoItem.Complete,
                sortOrder = rowCount,
            }, transaction: _transaction);
            ToDoLists toDoList = await _connection.QuerySingleAsync<ToDoLists>(
                "SELECT * FROM to_do_lists WHERE Id = @listId", new { listId = toDoItem.ListId }, transaction: _transaction);
            List<ToDoItems> listItems = (await _connection.QueryAsync<ToDoItems>(
                "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder, ToDoItem", new { listId = toDoItem.ListId }, transaction: _transaction)).ToList();
            return (true, false, toDoList, listItems);
        }

        public async Task<(bool success, bool wasConflict)> UpdateWithVersionCheck(ToDoItems toDoItem)
        {
            // Version check + bump on parent list
            string sqlBump = @"UPDATE to_do_lists SET Version = Version + 1 
                       WHERE Id = @listId AND Version = @listVersion";

            int bumped = await _connection.ExecuteAsync(sqlBump,
                new { listId = toDoItem.ListId, listVersion = toDoItem.ListVersion }, transaction: _transaction);

            // conflict
            if (bumped == 0) return (false, true);

            string sql = @"UPDATE to_do_items SET ToDoItem = @newToDoItem WHERE Id = @id";
            int rowCount = await _connection.ExecuteAsync(sql, new { newToDoItem = toDoItem.ToDoItem, id = toDoItem.Id }, transaction: _transaction);

            // failed to update but no conflict
            if (rowCount == 0) return (false, false);

            return (true, false);
        }

        public async Task<(bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems)> DeleteWithVersionCheck(long id, long listId, int expectedVersion)
        {
            // Version check + bump
            string sqlBump = @"UPDATE to_do_lists SET Version = Version + 1 
                       WHERE Id = @listId AND Version = @expectedVersion";
            int bumped = await _connection.ExecuteAsync(sqlBump, new { listId, expectedVersion }, transaction: _transaction);
            //conflict
            if(bumped == 0) 
            {
                ToDoLists current = await _connection.QuerySingleAsync<ToDoLists>(
                    "SELECT * FROM to_do_lists WHERE Id = @listId", new { listId }, transaction: _transaction);
                List<ToDoItems> currentItems = (await _connection.QueryAsync<ToDoItems>(
                    "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder, ToDoItem", new { listId }, transaction: _transaction)).ToList();
                return (false, true, current, currentItems);
            }
            string sql = @"DELETE FROM to_do_items WHERE Id = @id";
            await _connection.ExecuteAsync(sql, new { id }, transaction: _transaction);

            //get row count
            string countSql = @"SELECT COUNT(*) FROM to_do_items WHERE ListId = @listId";
            int rowCount = await _connection.QuerySingleAsync<int>(countSql, new { listId }, transaction: _transaction);
            int newMax = rowCount == 0 ? 0 : rowCount - 1;

            if (rowCount > 0)
            {
                //Ensure NO SortOrder value can exceed the maximum valid index after delete
                string sqlRenormalize = @"UPDATE to_do_items SET SortOrder = LEAST(SortOrder, @newMax) WHERE ListId = @ListId;";
                await _connection.ExecuteAsync(sqlRenormalize, new { newMax, listId }, transaction: _transaction);
            }
            

            ToDoLists toDoList = await _connection.QuerySingleAsync<ToDoLists>(
                "SELECT * FROM to_do_lists WHERE Id = @listId", new { listId }, transaction: _transaction);
            List<ToDoItems> listItems = (await _connection.QueryAsync<ToDoItems>(
                "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder, ToDoItem", new { listId }, transaction: _transaction)).ToList();

            return (true, false, toDoList, listItems);
        }
    }
}
