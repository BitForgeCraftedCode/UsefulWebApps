using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Models.ViewModels.MyHomePage;

namespace UsefulWebApps.Repository.IRepository
{
    public interface ISlideShowRepository : IRepository<SlideShowImages>
    {
        //any SlideShow specific database methods here
        Task<List<SlideShowImages>> GetSlideShowImagesForUser(string userId);
        Task<(SlideShowFolder userSlideShowFolder, List<SlideShowFolder> allSlideShowFolders)> GetSlideShowFoldersEditDisplay(string userId);
        Task<bool> UpdateSlideShow(string userId, SelectSlideShowVM selectSlideShowVM);
    }
}
