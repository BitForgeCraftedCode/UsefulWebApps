using Dapper;
using MySqlConnector;
using UsefulWebApps.DTO.ListBuddy;
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
                ToDoListViewState conflictViewState = await _helper.GetListViewState(toDoItem.ListId, _transaction);
                return (false, true, conflictViewState.ToDoList, conflictViewState.ListItems);
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

            ToDoListViewState viewState = await _helper.GetListViewState(toDoItem.ListId, _transaction);
            return (true, false, viewState.ToDoList, viewState.ListItems);
        }

        public async Task<(bool success, bool wasConflict)> UpdateWithVersionCheck(ToDoItems toDoItem)
        {
            // Version check + bump on parent list
            bool bumped = await _helper.TryBumpVersion(toDoItem.ListId, toDoItem.ListVersion, _transaction);
            // conflict
            if (!bumped) return (false, true);

            string sql = @"UPDATE to_do_items SET ToDoItem = @newToDoItem WHERE Id = @id";
            int rowCount = await _connection.ExecuteAsync(sql, new { newToDoItem = toDoItem.ToDoItem, id = toDoItem.Id }, transaction: _transaction);

            // failed to update but no conflict
            if (rowCount == 0) return (false, false);

            return (true, false);
        }

        public async Task<(bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems)> DeleteWithVersionCheck(long id, long listId, int expectedVersion)
        {
            // Version check + bump
            bool bumped = await _helper.TryBumpVersion(listId, expectedVersion, _transaction);
            //conflict
            if(!bumped) 
            {
                ToDoListViewState conflictViewState = await _helper.GetListViewState(listId, _transaction);
                return (false, true, conflictViewState.ToDoList, conflictViewState.ListItems);
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
            
            ToDoListViewState viewState = await _helper.GetListViewState(listId, _transaction);
            return (true, false, viewState.ToDoList, viewState.ListItems);
        }
    }
}
