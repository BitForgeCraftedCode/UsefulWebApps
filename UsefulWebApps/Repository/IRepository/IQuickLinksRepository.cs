using UsefulWebApps.Models.MyHomePage;
using UsefulWebApps.Models.ViewModels.MyHomePage;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IQuickLinksRepository : IRepository<QuickLinks>
    {
        //any QuickLink specific database methods here
        Task<List<QuickLinks>> GetQuickLinksForUser(string userId);
        Task<(List<QuickLinks> userQuickLinks, List<QuickLinks> allQuickLinks)> GetQuickLinksForEditDisplay(string userId);
        Task<bool> UpdateQuickLinks(string userId, string userName, SelectQuickLinksVM selectQuickLinksVM);
    }
}
