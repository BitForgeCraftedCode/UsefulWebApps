using Dapper;
using MySqlConnector;
using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Models.ViewModels.MyHomePage;
using UsefulWebApps.Repository.IRepository;
using static Dapper.SqlMapper;

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

        public async Task<(
           SlideShowFolder userSlideShowFolder,
           List<SlideShowFolder> allSlideShowFolders
           )> GetSlideShowFoldersEditDisplay(string userId)
        {
            string sqlMult = @"
                SELECT DISTINCT FolderName FROM slideshow_images WHERE SlideShowImageId IN (SELECT SlideShowImageId FROM user_slideshow_images WHERE UserId = @userId);
                SELECT DISTINCT FolderName FROM slideshow_images;
            ";

            GridReader gridReader = await _connection.QueryMultipleAsync(sqlMult, new { userId });
            //will be null if user has never picked a slideshow folder
            SlideShowFolder? userSlideShowFolder = await gridReader.ReadSingleOrDefaultAsync<SlideShowFolder>();
            List<SlideShowFolder> allSlideShowFolders = (List<SlideShowFolder>)await gridReader.ReadAsync<SlideShowFolder>();

            return (userSlideShowFolder, allSlideShowFolders);
        }

        public async Task<bool> UpdateSlideShow(string userId, SelectSlideShowVM selectSlideShowVM)
        {
            int rowsEffected1 = 0;
            int rowsEffected2 = 0;
            string folderName = selectSlideShowVM.SelectedSlideShowFolder.FolderName;
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            //delete all users slideshow choice then add the new selection
            string sql1 = @"DELETE FROM user_slideshow_images WHERE UserId = @userId"; //may return 0 if user has no slideshow selected
            string sql2 = @"
                INSERT INTO user_slideshow_images (UserId, UserName, SlideShowImageId) 
                SELECT aspnetusers.Id UserId, aspnetusers.UserName, slideshow_images.SlideShowImageId
                FROM slideshow_images
                CROSS JOIN aspnetusers
                WHERE slideshow_images.FolderName = @folderName AND aspnetusers.Id = @userId;
            ";
            rowsEffected1 = await _connection.ExecuteAsync(sql1, new { userId = userId }, transaction: txn);
            rowsEffected2 = await _connection.ExecuteAsync(sql2, new { userId = userId, folderName = folderName }, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return (rowsEffected1 + rowsEffected2 > 0 ? true : false);
        }
    }
}
