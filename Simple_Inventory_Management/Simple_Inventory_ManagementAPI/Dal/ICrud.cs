namespace Simple_Inventory_ManagementAPI.Dal
{
    public interface ICrud<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        T Add(T entity);
        T Update(T entity);
    }
}
