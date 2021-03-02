using CRMit.Customers.Models;
using Microsoft.EntityFrameworkCore;

namespace CRMit.Customers
{
    public class CustomersDbContext : DbContext
    {
        public CustomersDbContext(DbContextOptions<CustomersDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Customer> Customers { get; set; }
    }
}
