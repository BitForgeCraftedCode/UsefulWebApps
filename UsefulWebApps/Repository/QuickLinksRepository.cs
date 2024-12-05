using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

namespace UsefulWebApps.Repository
{
    public class QuickLinksRepository : Repository<QuickLinks> , IQuickLinksRepository
    {
        private readonly MySqlConnection _connection;

        public QuickLinksRepository(MySqlConnection db) : base(db)
        {
            _connection = db;
        }
        //any QuickLinks sepecific database methods here
    }
}
