﻿using CRMit.Customers.Models;
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
        public async Task<ActionResult<Customer>> CreateCustomerAsync([FromBody] CustomerDTO customer)
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

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetListAsync()
        {
            var customers = await context.Customers.ToListAsync();
            return new JsonResult(customers);
        }

        [HttpGet("{id}", Name = "GetCustomer")]
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
