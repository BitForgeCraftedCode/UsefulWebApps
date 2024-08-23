namespace UsefulWebApps.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IToDoListRepository ToDoList {  get; }
        //other repos here
    }
}
