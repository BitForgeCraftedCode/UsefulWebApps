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
        public async Task<(List<UserProfiles> profiles, List<Friendships> friendships)> GetFriendsWithProfiles(string userId)
        {
            /*
             * get the user profiles for all the userId's friends
             */
            string sql = @"SELECT up.*, f.Id, f.RequesterUserId, f.AddresseeUserId, f.Status, f.CreatedAt, f.UpdatedAt 
                   FROM friendships f
                   JOIN user_profiles up 
                        ON up.UserId = CASE
                            WHEN f.RequesterUserId = @userId THEN f.AddresseeUserId
                            ELSE f.RequesterUserId
                        END
                   WHERE (f.AddresseeUserId = @userId OR f.RequesterUserId = @userId) AND f.Status = 1";

            List<UserProfiles> profiles = new();
            List<Friendships> friendships = new();

            /* Dapper supports multi-mapping, which allows you to map a single row to multiple objects. 
             * For every row, Dapper splits the columns on "Id" — everything to the left maps to UserProfiles,
             * and "Id" plus everything to the right maps to Friendships. "Id" is the split point because 
             * the SELECT deliberately lists up.* first and then starts the friendship columns with f.Id.
             * The callback receives both mapped objects, adds them to their respective lists, and returns 
             * the UserProfiles object as required by the QueryAsync return type (unused here).
             */
            await _connection.QueryAsync<UserProfiles, Friendships, UserProfiles>(sql,
                (profile, friendship) =>
                {
                    profiles.Add(profile);
                    friendships.Add(friendship);
                    return profile;
                }, new { userId }, splitOn: "Id");

            return (profiles, friendships);
        }
        public async Task<(List<UserProfiles> profiles, List<Friendships> friendships)> GetPendingRequestsWithProfiles(string addresseeUserId)
        {
            /* CAUTION dont adjust sql without checking the Requests VIEW and VM -- UserFrofiles and PendingRequests are in correct order from DB and assumed to be that way in the view. 
             * get the user profiles for all the requestors that are pending for the addressee
             */
            string sql = @"SELECT up.*, f.Id, f.RequesterUserId, f.AddresseeUserId, f.Status, f.CreatedAt, f.UpdatedAt 
                   FROM friendships f
                   JOIN user_profiles up ON up.UserId = f.RequesterUserId
                   WHERE f.AddresseeUserId = @addresseeUserId AND f.Status = 0";

            List<UserProfiles> profiles = new();
            List<Friendships> friendships = new();

            /* Dapper supports multi-mapping, which allows you to map a single row to multiple objects. 
             * For every row, Dapper splits the columns on "Id" — everything to the left maps to UserProfiles,
             * and "Id" plus everything to the right maps to Friendships. "Id" is the split point because 
             * the SELECT deliberately lists up.* first and then starts the friendship columns with f.Id.
             * The callback receives both mapped objects, adds them to their respective lists, and returns 
             * the UserProfiles object as required by the QueryAsync return type (unused here).
             */
            await _connection.QueryAsync<UserProfiles, Friendships, UserProfiles>(sql,
                (profile, friendship) =>
                {
                    profiles.Add(profile);
                    friendships.Add(friendship);
                    return profile;
                }, new { addresseeUserId }, splitOn: "Id");

            return (profiles, friendships);
        }

        public async Task<bool> UpdateStatus(ulong friendshipId, FriendshipStatus status)
        {
            string sql = @"UPDATE friendships SET Status = @status WHERE Id = @friendshipId";
            int rows = await _connection.ExecuteAsync(sql, new { friendshipId, status });

            return rows > 0 ? true : false;
        }
    }
}
