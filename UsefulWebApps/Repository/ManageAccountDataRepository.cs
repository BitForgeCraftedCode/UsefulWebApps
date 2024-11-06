using Dapper;
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
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            int? rowsEffected1 = null;
            int? rowsEffected2 = null;
            int? rowsEffected3 = null;
            int? rowsEffected4 = null;
            int? rowsEffected5 = null;
           
            //delete recipe comments associated with user
            string sql1 = @"DELETE FROM recipe_comments WHERE UserId = @userId";
            //delete recipe user saved associated with user
            string sql2 = @"DELETE FROM recipe_usersaved WHERE UserId = @userId";
            //chage the UserName and UserId for each of the user's recipes to the admin's user name and id
            string sql3 = @"UPDATE recipes
                    SET UserId = @adminUserId, UserName = @adminUserName
                    WHERE UserId = @userId";
            //delete grocery list associated with user
            string sql4 = @"DELETE FROM grocery_list WHERE UserId = @userId";
            //delete to do list associated with user
            string sql5 = @"DELETE FROM to_do_list WHERE UserId = @userId";

            rowsEffected1 = await _connection.ExecuteAsync(sql1, new { userId = user.Id }, transaction: txn);
            rowsEffected2 = await _connection.ExecuteAsync(sql2, new { userId = user.Id }, transaction: txn);
            rowsEffected3 = await _connection.ExecuteAsync(sql3, new 
            { 
                adminUserId = admin.Id, 
                adminUserName = admin.UserName, 
                userId = user.Id 
            }, transaction: txn);
            rowsEffected4 = await _connection.ExecuteAsync(sql4, new { userId = user.Id }, transaction: txn);
            rowsEffected5 = await _connection.ExecuteAsync(sql5, new { userId = user.Id }, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();

            return (rowsEffected1 >= 0 && rowsEffected2 >= 0 && rowsEffected3 >= 0 && rowsEffected4 >= 0 && rowsEffected5 >= 0) ? true : false; ;
        }
    }
}
