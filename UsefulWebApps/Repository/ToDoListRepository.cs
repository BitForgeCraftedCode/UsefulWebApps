using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

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
    }
}
