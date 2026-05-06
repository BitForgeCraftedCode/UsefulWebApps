using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

namespace UsefulWebApps.Repository
{
    public class GroceryListItemsRepository : Repository<GroceryListItems>, IGroceryListItemsRepository
    {
        public GroceryListItemsRepository(MySqlConnection connection) : base(connection) { }
    }
}
