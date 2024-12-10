using UsefulWebApps.Models.MyHomePage;

namespace UsefulWebApps.Repository.IRepository
{
    public interface ISlideShowRepository : IRepository<SlideShowImages>
    {
        //any SlideShow specific database methods here
        Task<List<SlideShowImages>> GetSlideShowImagesForUser(string userId);
        Task<(SlideShowFolder userSlideShowFolder, List<SlideShowFolder> allSlideShowFolders)> GetSlideShowFoldersEditDisplay(string userId);
    }
}
