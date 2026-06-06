using UsefulWebApps.Models.MyHomePage;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IQuotesRepository : IRepository<Quotes>
    {
        //any Quote specific database methods here

        Task<Quotes?> GetRandomQuoteForUser(string userId);
        Task<List<Quotes>> GetQuotesForUser(string userId);
        Task<bool> DeleteQuoteForUser(long? quoteId, string userId);
    }
}
