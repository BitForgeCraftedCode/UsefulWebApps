using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

namespace UsefulWebApps.Repository
{
    public class QuotesRepository : Repository<Quotes>, IQuotesRepository
    {
        public QuotesRepository(MySqlConnection connection) : base(connection) { }
        //any Quotes specific database methods here
    }
}
