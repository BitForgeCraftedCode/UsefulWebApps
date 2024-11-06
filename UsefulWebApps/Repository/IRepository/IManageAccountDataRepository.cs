using Microsoft.AspNetCore.Identity;

namespace UsefulWebApps.Repository.IRepository
{
    //special class just to clean up user accout data not inheriting generic IRepository
    public interface IManageAccountDataRepository
    {
        Task<bool> DeleteUserData(IdentityUser user, IdentityUser admin);
    }
}
