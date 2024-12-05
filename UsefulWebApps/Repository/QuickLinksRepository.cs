using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;

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
        public async Task<List<QuickLinks>> GetQuickLinksForUser(string userId)
        {
            string sql = @"SELECT * FROM quick_links WHERE QuickLinkId IN (
                SELECT QuickLInkId FROM user_quick_links WHERE UserId = @userId
            );";

            List<QuickLinks> usersQuickLinks = (List<QuickLinks>)await _connection.QueryAsync<QuickLinks>(sql, new { userId });
            await _connection.CloseAsync();
            return usersQuickLinks;
        }
    }
}
