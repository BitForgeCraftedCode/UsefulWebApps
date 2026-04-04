using MySqlConnector;
using UsefulWebApps.Models.Friends;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class FriendshipsRepository : Repository<Friendships>, IFriendshipsRepository
    {
        public FriendshipsRepository(MySqlConnection connection) : base(connection) { }
        //any Friendships specific database methods here
    }
}
