using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Repository
{
    public class SlideShowRepository : Repository<SlideShowImages>, ISlideShowRepository
    {
        private readonly MySqlConnection _connection;

        public SlideShowRepository(MySqlConnection db) : base(db) 
        {
            _connection = db;
        }
        //any SlideShow specific database methods here
        public async Task<List<SlideShowImages>> GetSlideShowImagesForUser(string userId)
        {
            string sql = @"SELECT * FROM slideshow_images WHERE SlideShowImageId IN (
                SELECT SlideShowImageId FROM user_slideshow_images WHERE UserId = @userId
            )";

            List<SlideShowImages> userSlideShowImages = (List<SlideShowImages>)await _connection.QueryAsync<SlideShowImages>(sql, new { userId });
            await _connection.CloseAsync();
            return userSlideShowImages;
        }
    }
}
