namespace Simple_Inventory_ManagementMVC.Dal
{
    public interface ICrud<T> where T : class
    {
        IEnumerable<T> GetAll();
    }
}
