using Microsoft.EntityFrameworkCore;
using Simple_Inventory_ManagementMVC.Models;

namespace Simple_Inventory_ManagementMVC.Dal
{
    public class Products : IProduct
    {
        private readonly InventoryDBContext _dbContext;

        public Products(InventoryDBContext dbContext)
        {
            _dbContext = dbContext;    
        }
        public IEnumerable<Product> GetAll()
        {
            return _dbContext.Products
                .FromSqlRaw("SELECT ProductID, Name, StockLevel FROM Products")
                .ToList();
        }
    }
}
