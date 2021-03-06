using CRMit.Customers.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CRMit.Customers.IntegrationTests
{
    public class TestCustomers
    {
        [Test]
        public async Task TestCustomerCreated()
        {
            var connectionString = "Server=localhost;Port=3306;Database=CustomersDB;Uid=customers_service;Pwd=password";
            using (var context = new CustomersDbContext(
                new DbContextOptionsBuilder<CustomersDbContext>()
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .Options))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.json", optional: true)
                           .AddJsonFile("appsettings.Development.json", optional: true);
                })
                .UseStartup<Startup>());

            var client = server.CreateClient();

            var newCustomer = new CustomerDTO { Name = "Ivan", Surname = "Petrov", Email = "ivan.petrov@gmail.com" };
            var postContent = JsonContent.Create(newCustomer);
            var response = await client.PostAsync("/api/v1/customers/", postContent);

            response.EnsureSuccessStatusCode();

            response = await client.GetAsync(response.Headers.Location);

            response.EnsureSuccessStatusCode();
            var customer = await response.Content.ReadFromJsonAsync<Customer>();

            Assert.That(customer.Id, Is.EqualTo(1));
            Assert.That(customer.Name, Is.EqualTo(newCustomer.Name));
            Assert.That(customer.Surname, Is.EqualTo(newCustomer.Surname));
            Assert.That(customer.Email, Is.EqualTo(newCustomer.Email));
        }
    }
}