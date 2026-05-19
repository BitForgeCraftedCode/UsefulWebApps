using Dapper;
using MySqlConnector;
using UsefulWebApps.DTO.ListBuddy;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.Helpers;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class ToDoListsRepository : Repository<ToDoLists>, IToDoListsRepository
    {
        private readonly ToDoListRepositoryHelper _helper;
        public ToDoListsRepository(MySqlConnection connection) : base(connection) 
        {
            _helper = new ToDoListRepositoryHelper(connection);
        }

        //any ToDoLists model specific database methods here
        public async Task<List<ToDoItems>> GetAllItemsInList(long? listId)
        {
            string sql = "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder, ToDoItem";
            List<ToDoItems> items = (await _connection.QueryAsync<ToDoItems>(sql, new { listId })).ToList();
            return items;
        }

        //transaction method
        public async Task<(bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListToggleComplete(long id, long listId, int expectedVersion)
        {
            // Version check + bump
            bool bumped = await _helper.TryBumpVersion(listId, expectedVersion, _transaction);
            if (!bumped)
            {
                ToDoListViewState conflictViewState = await _helper.GetListViewState(listId, _transaction);
                return (false, true, conflictViewState.ToDoList, conflictViewState.ListItems);
            }
            // toggle
            bool isComplete = await _connection.QuerySingleAsync<bool>(
                "SELECT Complete FROM to_do_items WHERE Id = @id", new { id }, transaction: _transaction);
            string sqlToggle = isComplete
                ? "UPDATE to_do_items SET Complete = False WHERE Id = @id"
                : "UPDATE to_do_items SET Complete = True WHERE Id = @id";
            await _connection.ExecuteAsync(sqlToggle, new { id }, transaction: _transaction);
           
            ToDoListViewState viewState = await _helper.GetListViewState(listId, _transaction);
            return (true, false, viewState.ToDoList, viewState.ListItems);
        }

        //transaction method
        public async Task<(bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListSortItem(long id, long listId, int sortOrder, int expectedVersion)
        {
            // Version check + bump
            bool bumped = await _helper.TryBumpVersion(listId, expectedVersion, _transaction);
            if (!bumped)
            {
                ToDoListViewState conflictViewState = await _helper.GetListViewState(listId, _transaction);
                return (false, true, conflictViewState.ToDoList, conflictViewState.ListItems);
            }
            string sql = @"UPDATE to_do_items SET SortOrder = @sortOrder WHERE Id = @id";
            await _connection.ExecuteAsync(sql, new { sortOrder, id }, transaction: _transaction);
            
            ToDoListViewState viewState = await _helper.GetListViewState(listId, _transaction);
            return (true, false, viewState.ToDoList, viewState.ListItems);
        }
    }
}
