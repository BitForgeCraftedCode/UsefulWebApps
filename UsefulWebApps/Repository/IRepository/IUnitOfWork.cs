namespace UsefulWebApps.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IToDoListRepository ToDoList {  get; }
        IGroceryListRepository GroceryList { get; }
        //other repos here
    }
}
