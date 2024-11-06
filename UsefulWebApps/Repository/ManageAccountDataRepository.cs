using Microsoft.AspNetCore.Identity;
using MySqlConnector;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class ManageAccountDataRepository : IManageAccountDataRepository
    {
        private readonly MySqlConnection _connection;

        public ManageAccountDataRepository(MySqlConnection db)
        {
            _connection = db;
        }

        public async Task<bool> DeleteUserData(IdentityUser user, IdentityUser admin)
        {
            //do all this as a transaction
            //delete recipe comments associated with user

            //delete recipe user saved associated with user

            //chage the UserName and UserId for each of the user's recipes to the admin's user name and id

            //delete grocery list associated with user

            //delete to do list associated with user
            return true;
        }
    }
}
