using MySqlConnector;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class ToDoListsRepository : Repository<ToDoLists>, IToDoListsRepository
    {
        public ToDoListsRepository(MySqlConnection connection) : base(connection) { }

        //any ToDoLists model specific database methods here
    }
}
