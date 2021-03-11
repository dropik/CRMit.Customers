using CRMit.Customers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CRMit.Customers.Controllers
{
    [Route("crmit/v1/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomersDbContext context;

        public CustomersController(CustomersDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Create a new customer.
        /// </summary>
        /// <param name="customer">Customer data to be used for new customer.</param>
        /// <response code="201">Customer created and standard Created response returned with new customer.</response>
        [HttpPost("", Name = "CreateCustomer")]
        [ProducesResponseType(typeof(Customer), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Customer>> CreateCustomerAsync([FromBody, Required] CustomerInput customer)
        {
            var newCustomer = new Customer(customer);

            try
            {
                await context.AddAsync(newCustomer);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return CreatedAtAction("GetCustomer", new { id = newCustomer.Id }, newCustomer);
        }

        /// <summary>
        /// Get a list containing all customers.
        /// </summary>
        /// <response code="200">List with all customers.</response>
        [HttpGet("", Name = "GetCustomersList")]
        [ProducesResponseType(typeof(IEnumerable<Customer>), 200)]
        public async Task<ActionResult<IEnumerable<Customer>>> GetListAsync()
        {
            var customers = await context.Customers.ToListAsync();
            return new JsonResult(customers);
        }

        /// <summary>
        /// Get a customer by id.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <response code="200">Found customer.</response>
        [HttpGet("{id}", Name = "GetCustomer")]
        [ProducesResponseType(typeof(Customer), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Customer>> GetCustomerAsync(long id)
        {
            var result = await context.FindAsync<Customer>(id);
            if (result == null)
            {
                return NotFound();
            }
            return new JsonResult(result);
        }

        /// <summary>
        /// Update a customer given id.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="customer">Customer data to update with.</param>
        /// <response code="200">Customer updated.</response>
        [HttpPut("{id}", Name = "EditCustomer")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCustomerAsync(long id, [FromBody, Required] CustomerInput customer)
        {
            if (context.Customers.Any(c => c.Id == id))
            {
                var updatedCustomer = new Customer(customer) { Id = id };
                try
                {
                    context.Update(updatedCustomer);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// Delete a customer given id.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <response code="200">Customer deleted.</response>
        [HttpDelete("{id}", Name = "DeleteCustomer")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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
