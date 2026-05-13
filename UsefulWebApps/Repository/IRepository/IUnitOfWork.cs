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
        IToDoListsRepository ToDoLists { get; }
        IToDoItemsRepository ToDoItems { get; }
        IToDoListSharesRepository ToDoListShares { get; }
        IGroceryListsRepository GroceryLists { get; }
        IGroceryListItemsRepository GroceryListItems { get; }
        IGroceryListSharesRepository GroceryListShares { get; }
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
