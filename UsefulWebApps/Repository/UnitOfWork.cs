using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
//https://dotnettutorials.net/lesson/unit-of-work-csharp-mvc/
namespace UsefulWebApps.Repository
{
    //goal is to use UnitOfWork to share the _connection
    //this passes down one connection throught the entire inheritance chain
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MySqlConnection _connection;

        public IToDoListRepository ToDoList {  get; private set; }
        public IGroceryListRepository GroceryList { get; private set; }
        public IRecipeRepository Recipe { get; private set; }
        public IManageAccountDataRepository ManageAccountData { get; private set; }
        public INotesRepository Notes { get; private set; }
        public IQuickLinksRepository QuickLinks { get; private set; }
        public ISlideShowRepository SlideShow { get; private set; }
        public IQuotesRepository Quotes { get; private set; }
        public ILocationsRepository Locations { get; private set; }
        //other repos here

        public UnitOfWork(MySqlConnection db)
        {
            _connection = db;
            ToDoList = new  ToDoListRepository(_connection);
            GroceryList = new GroceryListRepository(_connection);
            Recipe = new RecipeRepository(_connection);
            ManageAccountData = new ManageAccountDataRepository(_connection);
            Notes = new NotesRepository(_connection);
            QuickLinks = new QuickLinksRepository(_connection);
            SlideShow = new SlideShowRepository(_connection);
            Quotes = new QuotesRepository(_connection);
            Locations = new LocationsRepository(_connection);
            //other repos here
        }
    }
}
