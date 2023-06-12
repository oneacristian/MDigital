using MDigital.Models;
using MDigital.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MDigital.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly MDigitalDBContext _context;

        public TransactionsController(MDigitalDBContext context)
        {
            _context = context;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            try
            {
                var transactions = await _context.Transactions
                .Include(t => t.Payment)
                .Include(t => t.Customer)
                .ToListAsync();

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving transactions: {ex.Message}");
            }
        }

        // GET: api/transactions/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(long id)
        {
            try
            {
                var transaction = await _context.Transactions
                .Include(t => t.Payment)
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t => t.Id == id);

                if (transaction == null)
                {
                    return NotFound();
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving transaction with ID - {id}: {ex.Message}");
            }
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(CreateTransactionDto dto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var transaction = new Transaction
                {
                    PaymentId = dto.PaymentId,
                    CustomerId = dto.CustomerId,
                };

                foreach (var articleDto in dto.Articles)
                {
                    var article = await _context.Articles.FindAsync(articleDto.ArticleId);
                    if (article == null)
                    {
                        // Article not found, return error response
                        return BadRequest($"Article with ID {articleDto.ArticleId} does not exist.");
                    }

                    // Create TransactionArticle entry
                    var transactionArticle = new TransactionArticle
                    {
                        ArticleId = article.Id,
                        ArticleQty = articleDto.ArticleQty,
                        Transaction = transaction
                    };

                    // Update the quantity of the article
                    article.Quantity -= articleDto.ArticleQty;

                    _context.TransactionArticles.Add(transactionArticle);
                }

                // Adding payments to the Transaction: 
                var payment = await _context.Payments.FindAsync(dto.PaymentId);
                if (payment == null)
                {
                    // Payment not found, return error response
                    return BadRequest($"Payment with ID {dto.PaymentId} does not exist.");
                }

                payment.Status = (int)Helpers.PaymentStatus.AttachedToTransaction;
                transaction.Payment = payment;

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the transaction: {ex.Message}");
            }
        }

        // PUT: api/transactions/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(long id, UpdateTransactionDto dto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var transaction = await _context.Transactions
                    .Include(t => t.Payment)
                    .Include(t => t.Customer)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (transaction == null)
                {
                    return NotFound();
                }

                // Update transaction properties
                transaction.PaymentId = dto.PaymentId;
                transaction.CustomerId = dto.CustomerId;

                // Update transaction articles
                foreach (var articleDto in dto.Articles)
                {
                    var transactionArticle = _context.TransactionArticles
                        .FirstOrDefault(ta => ta.ArticleId == articleDto.ArticleId && ta.TransactionId == id);

                    if (transactionArticle == null)
                    {
                        var article = await _context.Articles.FindAsync(articleDto.ArticleId);
                        if (article == null)
                        {
                            // Article not found, return error response
                            return BadRequest($"Article with ID {articleDto.ArticleId} does not exist.");
                        }

                        transactionArticle = new TransactionArticle
                        {
                            ArticleId = article.Id,
                            ArticleQty = articleDto.ArticleQty,
                            TransactionId = transaction.Id
                        };

                        _context.TransactionArticles.Add(transactionArticle);
                    }
                    else
                    {
                        // Update existing transaction article quantity
                        transactionArticle.ArticleQty = articleDto.ArticleQty;
                    }
                }
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException concurrException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception(concurrException.Message);
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the transaction: {ex.Message}");
            }
        }

        // DELETE: api/transactions/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(long id)
        {
            try
            {
                var transaction = await _context.Transactions.FindAsync(id);

                if (transaction == null)
                {
                    return NotFound();
                }

                // Deletion in cascade of the corresponding TransactionArticles:
                var jointedTransactionArticle = await _context.TransactionArticles.Where(ta => ta.TransactionId == id).ToListAsync();
                foreach(var transactionArticle in jointedTransactionArticle)
                    _context.TransactionArticles.Remove(transactionArticle);

                // Actual Transaction removal
                _context.Transactions.Remove(transaction);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the transaction: {ex.Message}");
            }
        }

        private bool TransactionExists(long id)
        {
            return _context.Transactions.Any(t => t.Id == id);
        }
    }

    public class CreateTransactionDto
    {
        [Required]
        public long PaymentId { get; set; }
        [Required]
        public long CustomerId { get; set; }
        [MinLength(1)]
        public List<ArticleDto> Articles { get; set; }
    }

    public class UpdateTransactionDto
    {
        [Required]
        public long PaymentId { get; set; }
        [Required]
        public long CustomerId { get; set; }
        [MinLength(1)]
        public List<ArticleDto> Articles { get; set; }
    }

    public class ArticleDto
    {
        public long ArticleId { get; set; }
        public int ArticleQty { get; set; }
    }
    public class PaymentDto
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
    }
}
