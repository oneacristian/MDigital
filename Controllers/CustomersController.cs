using MDigital.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MDigital.Controllers
{
    [ApiController]
    [Route("api/customers")]

    public class CustomersController : ControllerBase
    {
        private readonly MDigitalDBContext _context;

        public CustomersController(MDigitalDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            try
            {
                var customers = await _context.Customers.ToListAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving customers: {ex.Message}");
            }
        }

        // GET: api/customers/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(long id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the customer - {id}: {ex.Message}");
            }
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving the customer ({customer.Name}): {ex.Message}");
            }
        }

        // PUT: api/customers/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(long id, Customer customer)
        {
            try
            {
                if (id != customer.Id)
                {
                    return BadRequest();
                }

                _context.Entry(customer).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the customer ({customer.Name}): {ex.Message}");
            }
        }

        // DELETE: api/customers/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the customer with ID ({id}): {ex.Message}");
            }
        }

        private bool CustomerExists(long id)
        {
            return _context.Customers.Any(c => c.Id == id);
        }
    }
}
