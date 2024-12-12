namespace UsefulWebApps.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IToDoListRepository ToDoList {  get; }
        IGroceryListRepository GroceryList { get; }
        IRecipeRepository Recipe { get; }
        IManageAccountDataRepository ManageAccountData { get; }
        INotesRepository Notes { get; }
        IQuickLinksRepository QuickLinks { get; }
        ISlideShowRepository SlideShow { get; }
        IQuotesRepository Quotes { get; }
        //other repos here
    }
}
