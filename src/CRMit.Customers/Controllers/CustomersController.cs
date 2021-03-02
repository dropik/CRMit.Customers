using CRMit.Customers.Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace CRMit.Customers.Controllers
{
    [RoutePrefix("api/v1/[controller]")]
    public class CustomersController : ApiController
    {
        private readonly CustomersDbContext context;

        public CustomersController(CustomersDbContext context)
        {
            this.context = context;
        }

        [Route("")]
        public async Task<IHttpActionResult> ListAsync()
        {
            return default;
        }

        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetCustomerAsync(int id)
        {
            var result = await context.FindAsync<Customer>(id);
            return Json(result);
        }
    }
}
