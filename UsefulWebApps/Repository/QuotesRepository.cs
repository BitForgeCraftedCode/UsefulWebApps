using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;

namespace UsefulWebApps.Repository
{
    public class QuotesRepository : Repository<Quotes>, IQuotesRepository
    {
        public QuotesRepository(MySqlConnection connection) : base(connection) { }
        //any Quotes specific database methods here

        public async Task<Quotes?> GetRandomQuoteForUser(string userId)
        {
            string sql = @"SELECT * FROM quotes WHERE UserId = @userId ORDER BY RAND() LIMIT 1;";

            Quotes? quote = await _connection.QuerySingleOrDefaultAsync<Quotes>(sql, new { userId });

            return quote;
        }
    }
}
