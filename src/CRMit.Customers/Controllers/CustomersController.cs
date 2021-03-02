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
        public async Task<ActionResult<Customer>> CreateCustomerAsync(Customer customer)
        {
            if (customer.Id < 0)
            {
                return BadRequest();
            }

            try
            {
                await context.AddAsync(customer);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetCustomerAsync), new { id = customer.Id }, customer);
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
        public async Task<IActionResult> UpdateCustomerAsync(long id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            if (context.Customers.Any(c => c.Id == id))
            {
                context.Update(customer);
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
