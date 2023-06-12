using MDigital.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MDigital.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly MDigitalDBContext _context;

        public PaymentsController(MDigitalDBContext context)
        {
            _context = context;
        }

        // GET: api/payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            try
            {
                var payments = await _context.Payments.ToListAsync();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving payments: {ex.Message}");
            }
        }

        // GET: api/payments/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(long id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);

                if (payment == null)
                {
                    return NotFound();
                }

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the payment: {ex.Message}");
            }
        }

        // POST: api/payments
        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePayment([FromBody] CreatePaymentDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var payment = new Payment
                {
                    Date = paymentDto.Date,
                    Amount = paymentDto.Amount
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the payment: {ex.Message}");
            }
        }

        // PUT: api/payments/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(long id, [FromBody] UpdatePaymentDto paymentDto)
        {
            try
            {
                if (id != paymentDto.Id)
                {
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var payment = await _context.Payments.FindAsync(id);

                if (payment == null)
                {
                    return NotFound();
                }

                payment.Date = paymentDto.Date;
                payment.Amount = paymentDto.Amount;

                _context.Entry(payment).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the payment: {ex.Message}");
            }
        }

        // DELETE: api/payments/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(long id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);

                if (payment == null)
                {
                    return NotFound();
                }

                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the payment: {ex.Message}");
            }
        }

        private bool PaymentExists(long id)
        {
            return _context.Payments.Any(p => p.Id == id);
        }
    }

    public class CreatePaymentDto
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "The Amount must be a positive number and greater than 0.")]
        public decimal Amount { get; set; }
    }

    public class UpdatePaymentDto
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "The Amount must be a positive number and greater than 0.")]
        public decimal Amount { get; set; }
    }
}
