using MySqlConnector;
using UsefulWebApps.Models.Notifications;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class NotificationsRepository : Repository<Notifications>, INotificationsRepository
    {
        public NotificationsRepository(MySqlConnection connection) : base(connection) { }
        //any Notifications model specific database methods here
    }
}
