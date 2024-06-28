namespace Simple_Inventory_ManagementMVC.Models
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string TransactionType { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
    }
}
