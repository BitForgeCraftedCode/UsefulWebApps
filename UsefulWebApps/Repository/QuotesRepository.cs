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

        public async Task<List<Quotes>> GetQuotesForUser(string userId)
        {
            string sql = @"SELECT * FROM quotes WHERE UserId = @userId ORDER BY QuoteId DESC;";

            List<Quotes> quotes = (await _connection.QueryAsync<Quotes>(sql, new { userId })).ToList();

            return quotes;
        }
        public async Task<bool> DeleteQuoteForUser(long? quoteId, string userId)
        {
            string sql = @"DELETE FROM quotes WHERE QuoteId = @quoteId AND UserId = @userId;";

            int rowsEffected = await _connection.ExecuteAsync(sql, new { quoteId, userId });

            return rowsEffected > 0;
        }
    }
}
