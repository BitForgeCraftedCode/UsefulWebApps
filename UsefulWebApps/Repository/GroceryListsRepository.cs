using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

namespace UsefulWebApps.Repository
{
    public class GroceryListsRepository : Repository<GroceryLists>, IGroceryListsRepository
    {
        public GroceryListsRepository(MySqlConnection connection) : base(connection) { }
        //any GroceryList model specific database methods here
    }
}
