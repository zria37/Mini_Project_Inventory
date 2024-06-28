using Simple_Inventory_ManagementAPI.Models;

namespace Simple_Inventory_ManagementAPI.Dal
{
    public interface ITransaction : ICrud<Transaction>
    {
        IEnumerable<TransactionViewModel> GetProductTransact();
    }
}
