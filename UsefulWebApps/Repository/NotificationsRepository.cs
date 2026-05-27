using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.Notifications;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class NotificationsRepository : Repository<Notifications>, INotificationsRepository
    {
        public NotificationsRepository(MySqlConnection connection) : base(connection) { }
        //any Notifications model specific database methods here
        public async Task<int> GetUnreadCount(string userId) 
        {
            string sql = @"SELECT COUNT(*)
                   FROM notifications
                   WHERE UserId = @userId
                   AND IsRead = 0";

            int count = await _connection.ExecuteScalarAsync<int>(sql, new { userId });

            return count;
        }
    }
}
