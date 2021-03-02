using CRMit.Customers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetListAsync()
        {
            var customers = await context.Customers.ToListAsync();
            return new JsonResult(customers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Customer>> GetCustomerAsync(int id)
        {
            if (id < 1)
            {
                return new BadRequestResult();
            }
            var result = await context.FindAsync<Customer>(id);
            if (result == null)
            {
                return new NotFoundResult();
            }
            return new JsonResult(result);
        }
    }
}
