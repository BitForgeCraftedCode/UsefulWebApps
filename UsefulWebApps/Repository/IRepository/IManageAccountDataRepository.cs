using Microsoft.AspNetCore.Identity;
using MySqlConnector;
namespace UsefulWebApps.Repository.IRepository
{
    //special class just to clean up user accout data not inheriting generic IRepository
    public interface IManageAccountDataRepository
    {
        void SetTransaction(MySqlTransaction? txn);
        Task<bool> DeleteUserData(IdentityUser user, IdentityUser admin);
    }
}
