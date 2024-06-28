// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Simple_Inventory_Management.Models;

// untuk menginjek database
var serviceProvider = new ServiceCollection()
       .AddDbContext<InventoryDBContext>(options  => options.UseSqlServer("InventoryDB"))
       .BuildServiceProvider();

var dbContext = serviceProvider.GetRequiredService<InventoryDBContext>();

while (true)
{
    Console.WriteLine("Choose an option:");
    Console.WriteLine("1. Add Product");
    Console.WriteLine("2. Update Product");
    Console.WriteLine("3. Display Products");
    Console.WriteLine("4. Exit");
    Console.Write("Select Number an option: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddProduct();
            break;
        case "2":
            UpdateProduct();
            break;
        case "3":
            DisplayProducts();
            break;
        case "4":
            return;
        default:
            Console.WriteLine("Invalid choice");
            Pause();
            break;
    }
}

void Pause()
{
    Console.WriteLine("Press any key to return to the main menu...");
    Console.ReadKey(true);
    Console.WriteLine();
}

void DisplayProducts()
{
    var query = @"
        select p.ProductID, 
	           p.Name, 
	           StockLevel,
	           ISNULL(SUM(t.Quantity),0) TQuantity,
	           (StockLevel - ISNULL(SUM(t.Quantity),0))Hasil
        from Products p
        left join Transactions t on t.ProductID = p.ProductID
        where t.TransactionType = 'Add' 
	        OR t.TransactionID IS NULL
        GROUP BY p.ProductID, p.Name, StockLevel;";

    var products = dbContext.ProductResults
        .FromSqlRaw(query)
        .ToList();

    foreach (var product in products)
    {
        Console.WriteLine();
        Console.WriteLine("Informasi Data Produk");
        Console.WriteLine("---------------------------------------");
        Console.WriteLine($"{"Product Id :".PadRight(25)}{product.ProductId}");
        Console.WriteLine($"{"Product Name :".PadRight(25)}{product.Name}");
        Console.WriteLine($"{"Product Stock:".PadRight(25)}{product.StockLevel}");
        Console.WriteLine($"{"Total Quantity:".PadRight(25)}{product.TQuantity}");
        Console.WriteLine($"{"Hasil:".PadRight(25)}{product.Hasil}");
        Console.WriteLine("---------------------------------------");
        Console.WriteLine();
    }
}

void UpdateProduct()
{
    Console.Write("Enter Product ID: ");
    var productId = int.Parse(Console.ReadLine());

    var product = dbContext.Products.FirstOrDefault(p => p.ProductId == productId);
    if (product == null)
    {
        Console.WriteLine("Product not found!");
        return;
    }

    Console.Write("Enter Product Name: ");
    product.Name = Console.ReadLine();

    Console.Write("Enter Stock Level: ");
    product.StockLevel = int.Parse(Console.ReadLine());

    dbContext.SaveChanges();
    Console.WriteLine("Product updated successfully!");
    Console.WriteLine();
}

void AddProduct()
{
    Console.Write("Enter Product Name: ");
    var name = Console.ReadLine();

    Console.Write("Enter Stock Level: ");
    var stockLevel = int.Parse(Console.ReadLine());

    var product = new Product {Name = name, StockLevel = stockLevel };
    dbContext.Products.Add(product);
    dbContext.SaveChanges();
    Console.WriteLine("Product added successfully!");
    Console.WriteLine();
}