using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

namespace UsefulWebApps.Repository
{
    public class GroceryListRepository : Repository<GroceryList>, IGroceryListRepository
    {
        private readonly MySqlConnection _connection;
        public GroceryListRepository(MySqlConnection db) : base(db)
        {
            _connection = db;
        }

        //any GroceryList model specific database methods here
    }
}
