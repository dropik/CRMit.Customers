using CRMit.Customers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMit.Customers.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomersDbContext context;

        public CustomersController(CustomersDbContext context)
        {
            this.context = context;
        }

        [HttpPost("")]
        public async Task<ActionResult<Customer>> CreateCustomerAsync([FromBody]  CustomerDTO customer)
        {
            var newCustomer = new Customer(customer);

            await context.AddAsync(newCustomer);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomerAsync), new { id = newCustomer.Id }, newCustomer);
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetListAsync()
        {
            var customers = await context.Customers.ToListAsync();
            return new JsonResult(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerAsync(long id)
        {
            var result = await context.FindAsync<Customer>(id);
            if (result == null)
            {
                return NotFound();
            }
            return new JsonResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerAsync(long id, [FromBody] CustomerDTO customer)
        {
            if (context.Customers.Any(c => c.Id == id))
            {
                var updatedCustomer = new Customer(customer) { Id = id };
                context.Update(updatedCustomer);
                await context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerAsync(long id)
        {
            var customer = await context.FindAsync<Customer>(id);
            if (customer == null)
            {
                return NotFound();
            }

            context.Remove(customer);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
