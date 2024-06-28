using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Simple_Inventory_ManagementAPI.Models;

namespace Simple_Inventory_ManagementAPI.Dal
{
    public class Transactions : ITransaction
    {
        private InventoryDBContext _dbContext;

        public Transactions(InventoryDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Transaction Add(Transaction entity)
        {
            try
            {
                var sql = "INSERT INTO Transactions (ProductId, TransactionType, Quantity, Date) " +
                          "VALUES (@ProductId, @TransactionType, @Quantity, @Date); " +
                          "SELECT CAST(scope_identity() AS int);";

                var parameters = new[]
                {
                new SqlParameter("@ProductId", entity.ProductId),
                new SqlParameter("@TransactionType", entity.TransactionType),
                new SqlParameter("@Quantity", entity.Quantity),
                new SqlParameter("@Date", entity.Date)
            };

                var newTransactionId = _dbContext.Database.ExecuteSqlRaw(sql, parameters);
                entity.TransactionId = newTransactionId;

                _dbContext.Entry(entity).Reload(); // Reload to get any changes made by triggers
                return entity;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public IEnumerable<Transaction> GetAll()
        {
            return _dbContext.Transactions.Include(t => t.Product).ToList();
        }

        public Transaction GetById(int id)
        {
            var result = _dbContext.Transactions.Include(t => t.Product).SingleOrDefault(t => t.TransactionId == id);
            if (result == null)
            {
                throw new Exception("Transaction not Found");
            }
            return result;
        }

        public IEnumerable<TransactionViewModel> GetProductTransact()
        {
            var result = _dbContext.Transactions
                            .Include(t => t.Product)
                            .Select(t => new TransactionViewModel
                            {
                                TransactionId = t.TransactionId,
                                ProductId = t.ProductId,
                                ProductName = t.Product.Name,
                                TransactionType = t.TransactionType,
                                Quantity = t.Quantity,
                                Date = t.Date
                            })
                            .ToList();
            return result;
        }

        public Transaction Update(Transaction entity)
        {
            try
            {
                var existingEntity = _dbContext.Transactions.AsNoTracking().FirstOrDefault(t => t.TransactionId == entity.TransactionId);
                if (existingEntity == null)
                {
                    throw new ArgumentException("Transaction not found");
                }

                _dbContext.Entry(entity).State = EntityState.Modified;
                _dbContext.SaveChanges();
                // Reload entity to get updated values after insert (if necessary)
                _dbContext.Entry(entity).Reload();
                return entity;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

    }
}
