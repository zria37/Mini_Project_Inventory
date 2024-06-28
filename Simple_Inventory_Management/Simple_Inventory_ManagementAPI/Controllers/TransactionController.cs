using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Simple_Inventory_ManagementAPI.Dal;
using Simple_Inventory_ManagementAPI.Models;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace Simple_Inventory_ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransaction _transactionService;
        private readonly InventoryDBContext _dbContext;

        public TransactionController(ITransaction transactionService, InventoryDBContext dbContext)
        {
            _transactionService = transactionService;
            _dbContext = dbContext;
        }

        // GET: api/<TransactionController>
        [HttpGet]
        public ActionResult<IEnumerable<TransactionViewModel>> Get()
        {
            BackgroundJob.Enqueue(() => ExecuteGetTransactionsJob());

            var results = _transactionService.GetProductTransact();
            return Ok(results);
        }

        [NonAction]
        public void ExecuteGetTransactionsJob()
        {
            var transactions = _transactionService.GetProductTransact();
            foreach (var transaction in transactions)
            {
                Console.WriteLine($"Transaction: {transaction.ProductName}, {transaction.TransactionType}, {transaction.Quantity}, {transaction.Date}");
            }
        }

        // POST api/<TransactionController>
        [HttpPost]
        public IActionResult Post(Transaction transaction)
        {
            try
            {
                // Validate ProductId
                var product = _dbContext.Products.SingleOrDefault(p => p.ProductId == transaction.ProductId);
                if (product == null)
                {
                    return BadRequest("Invalid ProductId.");
                }

                // Set Product reference
                transaction.Product = product;

                var result = _transactionService.Add(transaction);

                var transactionViewModel = new TransactionViewModel
                {
                    TransactionId = result.TransactionId,
                    ProductId = result.ProductId,
                    ProductName = result.Product.Name,
                    TransactionType = result.TransactionType,
                    Quantity = result.Quantity,
                    Date = result.Date
                };
                BackgroundJob.Enqueue(() => ExecutePostTransactionJob(result.TransactionId));
                return CreatedAtAction(nameof(GetById), new { id = result.TransactionId }, transactionViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        public void ExecutePostTransactionJob(int transactionId)
        {
            var transaction = _transactionService.GetById(transactionId);
            if (transaction != null)
            {
                Console.WriteLine($"Processing transaction: {transaction.Product.Name}, {transaction.TransactionType}, {transaction.Quantity}, {transaction.Date}");
                // Additional processing logic can be added here
            }
        }



        // PUT api/<TransactionController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (id != transaction.TransactionId)
                {
                    return BadRequest("Transaction ID mismatch.");
                }

                var existingTransaction = _transactionService.GetById(id);
                if (existingTransaction == null)
                {
                    return NotFound("Transaction not found.");
                }

                existingTransaction.TransactionType = transaction.TransactionType;
                existingTransaction.Quantity = transaction.Quantity;
                existingTransaction.Date = transaction.Date;
                BackgroundJob.Enqueue(() => ExecutePutTransactionJob(existingTransaction.TransactionId));
                var updatedTransaction = _transactionService.Update(existingTransaction);

                var transactionViewModel = new TransactionViewModel
                {
                    TransactionId = updatedTransaction.TransactionId,
                    ProductId = updatedTransaction.ProductId,
                    ProductName = updatedTransaction.Product.Name,
                    TransactionType = updatedTransaction.TransactionType,
                    Quantity = updatedTransaction.Quantity,
                    Date = updatedTransaction.Date
                };

                return Ok(transactionViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        public void ExecutePutTransactionJob(int transactionId)
        {
            var transaction = _transactionService.GetById(transactionId);
            if (transaction != null)
            {
                Console.WriteLine($"Processing updated transaction: {transaction.Product.Name}, {transaction.TransactionType}, {transaction.Quantity}, {transaction.Date}");
                // Additional processing logic can be added here
            }
        }

        // GET api/<TransactionController>/5
        [HttpGet("{id}")]
        public ActionResult<TransactionViewModel> GetById(int id)
        {
            var transaction = _transactionService.GetById(id);
            if (transaction == null)
            {
                return NotFound("Transaction not found.");
            }

            var transactionViewModel = new TransactionViewModel
            {
                TransactionId = transaction.TransactionId,
                ProductId = transaction.ProductId,
                ProductName = transaction.Product.Name,
                TransactionType = transaction.TransactionType,
                Quantity = transaction.Quantity,
                Date = transaction.Date
            };

            return Ok(transactionViewModel);
        }
    }
}
