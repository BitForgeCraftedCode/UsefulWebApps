namespace UsefulWebApps.Repository.IRepository
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        Task OpenConnectionAsync();
        Task BeginTxnAsync();
        Task CommitAsync();
        Task RollbackAsync();
        ValueTask DisposeAsync();
        void Dispose();

        // Repositories
        IToDoListRepository ToDoList {  get; }
        IToDoListsRepository ToDoLists { get; }
        IToDoListSharesRepository ToDoListShares { get; }
        IGroceryListRepository GroceryList { get; }
        IRecipeRepository Recipe { get; }
        IManageAccountDataRepository ManageAccountData { get; }
        INotesRepository Notes { get; }
        INoteSharesRepository NoteShares { get; }
        IQuickLinksRepository QuickLinks { get; }
        ISlideShowRepository SlideShow { get; }
        IQuotesRepository Quotes { get; }
        ILocationsRepository Locations { get; }
        ICalendarEventsRepository CalendarEvents { get; }
        IFriendshipsRepository Friendships { get; }
        IUserProfilesRepository UserProfiles { get; }
    }
}
