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
        //other repos here
    }
}
