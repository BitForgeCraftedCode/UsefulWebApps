using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
//https://dotnettutorials.net/lesson/unit-of-work-csharp-mvc/
namespace UsefulWebApps.Repository
{
    // UnitOfWork – Design Summary
    /*
        UnitOfWork – Design Summary

        The UnitOfWork class centralizes database connection management, transaction control, 
        and repository coordination into a single cohesive abstraction.

        Core Responsibilities

        1. Shared Connection Ownership
        UnitOfWork owns a single MySqlConnection instance and injects it into all repositories 
        via the UnitOfWork constructor.

        This ensures:
        - One logical connection per request scope
        - Consistent connection lifecycle management
        - No accidental multi-connection usage across repositories
        - Proper connection pooling behavior (connections are returned to the pool, not destroyed)

        2. Centralized Transaction Management
        Transactions are created, committed, and rolled back only by UnitOfWork.
        Repositories do not create transactions themselves — they simply participate in one if provided.

        Transactions are explicit and opt-in (only exist when BeginTxnAsync() is called).

        3. Transaction Propagation Model
        When a transaction is started:
        The active MySqlTransaction is injected into all repositories via SetTransaction(...).

        This allows:
        - Transactional consistency across multiple repositories
        - Atomic multi-repository operations
        - Optional transactional behavior (repos work with or without a transaction)

        4. Lifecycle Control
        UnitOfWork manages:
        - Connection open/close
        - Transaction lifecycle
        - Resource cleanup

        via:
        - DisposeAsync() (primary)
        - Dispose() (sync fallback safety)

        This prevents:
        - Connection leaks
        - Transaction leaks
        - Pool exhaustion
        - Dangling unmanaged resources

        Note:
        ASP.NET Core DI AddScoped manages lifetime and calls DisposeAsync when the request scope ends.

        Usage in Controllers:

            Non-transaction methods:
                await _unitOfWork.RepoA.Method(); 
                // Dapper automatically opens/closes the connection

            Transaction methods:
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                await _unitOfWork.RepoA.Method();
                await _unitOfWork.CommitAsync();        
     */
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable, IDisposable
    {
        private readonly MySqlConnection _connection;
        private MySqlTransaction? _transaction;
        private bool _disposed = false;

        public IToDoListRepository ToDoList {  get; private set; }
        public IGroceryListRepository GroceryList { get; private set; }
        public IRecipeRepository Recipe { get; private set; }
        public IManageAccountDataRepository ManageAccountData { get; private set; }
        public INotesRepository Notes { get; private set; }
        public IQuickLinksRepository QuickLinks { get; private set; }
        public ISlideShowRepository SlideShow { get; private set; }
        public IQuotesRepository Quotes { get; private set; }
        public ILocationsRepository Locations { get; private set; }
        public ICalendarEventsRepository CalendarEvents { get; private set; }
        public IFriendshipsRepository Friendships { get; private set; }
        public IUserProfilesRepository UserProfiles { get; private set; }
        public INoteSharesRepository NoteShares { get; private set; }
        //other repos here

        public UnitOfWork(MySqlConnection connection)
        {
            _connection = connection;

            // Initialize repositories
            ToDoList = new  ToDoListRepository(_connection);
            GroceryList = new GroceryListRepository(_connection);
            Recipe = new RecipeRepository(_connection);
            ManageAccountData = new ManageAccountDataRepository(_connection);
            Notes = new NotesRepository(_connection);
            QuickLinks = new QuickLinksRepository(_connection);
            SlideShow = new SlideShowRepository(_connection);
            Quotes = new QuotesRepository(_connection);
            Locations = new LocationsRepository(_connection);
            CalendarEvents = new CalendarEventsRepository(_connection);
            Friendships = new FriendshipsRepository(_connection);
            UserProfiles = new UserProfilesRepository(_connection);
            NoteShares = new NoteSharesRepository(_connection);
        }

        public async Task OpenConnectionAsync()
        {
            if (_connection.State != System.Data.ConnectionState.Open)
                await _connection.OpenAsync();
        }
        public async Task BeginTxnAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("Transaction already started.");

            _transaction = await _connection.BeginTransactionAsync();
            PropagateTransaction(_transaction);
        }
        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            await _transaction.CommitAsync();
            _transaction = null;
            PropagateTransaction(null);
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to rollback.");

            await _transaction.RollbackAsync();
            _transaction = null;
            PropagateTransaction(null);
        }

        // Propagate transaction to all repositories
        private void PropagateTransaction(MySqlTransaction? txn)
        {
            ToDoList.SetTransaction(txn);
            GroceryList.SetTransaction(txn);
            Recipe.SetTransaction(txn);
            ManageAccountData.SetTransaction(txn);
            Notes.SetTransaction(txn);
            QuickLinks.SetTransaction(txn);
            SlideShow.SetTransaction(txn);
            Quotes.SetTransaction(txn);
            Locations.SetTransaction(txn);
            CalendarEvents.SetTransaction(txn);
        }

        // Async disposal -- DisposeAsync is called automatically by ASP.NET Core DI AddScoped
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            _disposed = true;
            //DisposeAsync() -> calls CloseAsync() -> returns to pool -> frees managed resources
            if (_transaction != null)
                await _transaction.DisposeAsync();

            if (_connection != null)
                await _connection.DisposeAsync();
        }

        // Sync disposal (fallback safety)
        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
