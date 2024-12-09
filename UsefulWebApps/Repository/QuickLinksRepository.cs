using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;
using static Dapper.SqlMapper;
using UsefulWebApps.Models.ViewModels.MyHomePage;

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

        public async Task<bool> UpdateQuickLinks(string userId, string userName, SelectQuickLinksVM selectQuickLinksVM)
        {
            int rowsEffected1 = 0;
            int rowsEffected2 = 0;
            //make a checked quick links parameter list for sql INSERT
            List<Object> checkedQuickLinksParams = new List<Object>();
            foreach (QuickLinks ql in selectQuickLinksVM.AllQuickLinks)
            {
                if(ql.IsSelected == true)
                {
                    checkedQuickLinksParams.Add(
                        new {userId = userId, userName = userName, quickLinkId = ql.QuickLinkId }    
                    );
                }
            }
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            //delete all users links then add the new selection
            string sql1 = @"DELETE FROM user_quick_links WHERE UserId = @userId"; //may return 0 if user has no links selected
            string sql2 = @"INSERT INTO user_quick_links (UserId, UserName, QuickLinkId) VALUES (@userId, @userName, @quickLinkId)";
            rowsEffected1 = await _connection.ExecuteAsync(sql1, new { userId = userId}, transaction: txn); 
            rowsEffected2 = await _connection.ExecuteAsync(sql2, checkedQuickLinksParams, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return (rowsEffected1 + rowsEffected2 > 0 ? true : false);
        }
    }
}
