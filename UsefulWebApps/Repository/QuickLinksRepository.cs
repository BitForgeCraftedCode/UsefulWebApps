using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;
using static Dapper.SqlMapper;

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
            ) ORDER BY Category;";

            List<QuickLinks> usersQuickLinks = (List<QuickLinks>)await _connection.QueryAsync<QuickLinks>(sql, new { userId });
            await _connection.CloseAsync();
            return usersQuickLinks;
        }

        public async Task<(
            List<QuickLinks> userQuickLinks, 
            List<QuickLinks> allQuickLinks
            )> GetQuickLinksForEditDisplay(string userId) 
        {
            string sqlMult = @"
                SELECT * FROM quick_links WHERE QuickLinkId IN (SELECT QuickLInkId FROM user_quick_links WHERE UserId = @userId);
                SELECT * FROM quick_links;
            ";

            GridReader gridReader = await _connection.QueryMultipleAsync(sqlMult, new { userId });
            List<QuickLinks> userQuickLinks = (List<QuickLinks>)await gridReader.ReadAsync<QuickLinks>();
            List<QuickLinks> allQuickLinks = (List<QuickLinks>)await gridReader.ReadAsync<QuickLinks>();

            return (userQuickLinks, allQuickLinks);
        }
    }
}
