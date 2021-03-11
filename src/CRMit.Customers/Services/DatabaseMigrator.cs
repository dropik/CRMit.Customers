using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRMit.Customers.Services
{
    public class DatabaseMigrator : IStartupTask
    {
        private readonly IServiceProvider serviceProvider;

        public DatabaseMigrator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync()
        {
            using var scope = serviceProvider.CreateScope();
            while (true)
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();
                    await context.Database.MigrateAsync();
                    Console.WriteLine("Connection with MySql established.");
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Connection to MySql failed. Retrying...");
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
