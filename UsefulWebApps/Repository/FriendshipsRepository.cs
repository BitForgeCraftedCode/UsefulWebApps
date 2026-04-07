using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.Friends;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class FriendshipsRepository : Repository<Friendships>, IFriendshipsRepository
    {
        public FriendshipsRepository(MySqlConnection connection) : base(connection) { }
        //any Friendships specific database methods here
        public async Task<Friendships?> GetExisting(string userId1, string userId2)
        {
            //need to check both directions A friended B or B friended A this way you cannot send a duplicate in the other direction
            string sql = @"SELECT * FROM friendships 
                   WHERE (RequesterUserId = @userId1 AND AddresseeUserId = @userId2)
                   OR (RequesterUserId = @userId2 AND AddresseeUserId = @userId1)";

            Friendships? friendship = await _connection.QueryFirstOrDefaultAsync<Friendships>(sql, new
            {
                userId1,
                userId2
            });

            return friendship;
        }
    }
}
