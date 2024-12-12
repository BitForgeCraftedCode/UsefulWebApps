using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

namespace UsefulWebApps.Repository
{
    public class QuotesRepository : Repository<Quotes>, IQuotesRepository
    {
        private readonly MySqlConnection _connection;

        public QuotesRepository(MySqlConnection db) : base(db) 
        {
            _connection = db;
        }
        //any Quotes specific database methods here
    }
}
