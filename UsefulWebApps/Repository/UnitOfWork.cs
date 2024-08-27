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
        //other repos here

        public UnitOfWork(MySqlConnection db)
        {
            _connection = db;
            ToDoList = new  ToDoListRepository(_connection);
            GroceryList = new GroceryListRepository(_connection);
            //other repos here
        }
    }
}
