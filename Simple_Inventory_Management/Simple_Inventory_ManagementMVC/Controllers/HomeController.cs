using Microsoft.AspNetCore.Mvc;
using Simple_Inventory_ManagementMVC.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Simple_Inventory_ManagementMVC.Dal;


namespace Simple_Inventory_ManagementMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        private readonly ILogger<HomeController> _logger;
        private readonly IProduct _context;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory, IProduct context)
        {
            _logger = logger;
            _context = context;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7111/api/Transaction");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var transactions = JsonConvert.DeserializeObject<List<TransactionViewModel>>(json);
                // Ambil data produk untuk dropdown
                var products =  _context.GetAll();
                ViewBag.Products = products;
                return View(transactions);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error fetching data");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionViewModel model)
        {
                try
                {
                    var client = _clientFactory.CreateClient();
                    var transactionDto = new
                    {
                        ProductId = model.ProductId,
                        TransactionType = model.TransactionType,
                        Quantity = model.Quantity,
                        Date = model.Date
                    };
                    var json = JsonConvert.SerializeObject(transactionDto);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("https://localhost:7111/api/Transaction", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Transaction created successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to create transaction.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating transaction");
                    TempData["ErrorMessage"] = "An error occurred while creating transaction.";
                }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
