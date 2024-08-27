namespace UsefulWebApps.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T -- ToDoList or any model/class we want to interact with dbContext
        //that is why T is a generic class
        //where T : class (just means T is a class)

        //https://stackoverflow.com/questions/14458566/making-interface-implementations-async
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<bool> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(int? id);

        Task<bool> DeleteAll();
    }
}
