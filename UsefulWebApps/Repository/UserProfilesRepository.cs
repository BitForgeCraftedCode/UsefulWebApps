using MySqlConnector;
using UsefulWebApps.Models.Friends;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class UserProfilesRepository : Repository<UserProfiles>, IUserProfilesRepository
    {
        public UserProfilesRepository(MySqlConnection connection) : base(connection) { }
        //any UserProfiles sepecific database methods here
    }
}
