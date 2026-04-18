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

        //transaction method
        public async Task<(ToDoLists toDoList, List<ToDoItems> listItems)> ToDoListAddItem(ToDoItems toDoItem)
        {
            //add the new item
            string sql = @"INSERT INTO to_do_items (ListId, ToDoItem, Complete, SortOrder) VALUES (@listId, @toDoItem, @complete, @sortOrder)";
            await _connection.ExecuteAsync(sql, new 
            { 
                listId = toDoItem.ListId,
                toDoItem = toDoItem.ToDoItem,
                complete = toDoItem.Complete,
                sortOrder = toDoItem.SortOrder,
            }, transaction: _transaction);
            //get the list
            string sql1 = "SELECT * FROM to_do_lists WHERE Id = @listId";
            ToDoLists toDoList = await _connection.QuerySingleAsync<ToDoLists>(sql1, new { listId = toDoItem.ListId }, transaction: _transaction);
            //get all list tiems for listId
            string sql2 = "SELECT * FROM to_do_items WHERE ListId = @listId ORDER BY SortOrder";
            List<ToDoItems> listItems = (List<ToDoItems>)await _connection.QueryAsync<ToDoItems>(sql2, new { listId = toDoItem.ListId }, transaction: _transaction);
            return (toDoList, listItems);
        }
    }
}
