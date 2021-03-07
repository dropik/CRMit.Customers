using CRMit.Customers.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CRMit.Customers.IntegrationTests
{
    [TestFixture]
    public class CustomersIT
    {
        private readonly HttpClient client;

        public CustomersIT()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.json", optional: true)
                           .AddJsonFile("appsettings.Development.json", optional: true);
                })
                .UseStartup<Startup>());

            client = server.CreateClient();
        }

        [SetUp]
        public async Task Setup()
        {
            var connectionString = "Server=localhost;Port=3306;Database=CustomersDB;Uid=customers_service;Pwd=password";
            using var context = new CustomersDbContext(
                new DbContextOptionsBuilder<CustomersDbContext>()
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .Options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        [Test]
        public async Task Test_CreateCustomer_AndGetCustomer_GivenReturnedLocation()
        {
            (var customer, var response) = await PostIvanPetrov();
            await EnsureCustomerCreated(customer, response);
        }

        private async Task<(CustomerDTO customer, HttpResponseMessage response)> PostIvanPetrov()
        {
            var customer = new CustomerDTO { Name = "Ivan", Surname = "Petrov", Email = "ivan@gmail.com" };
            var response = await client.PostAsJsonAsync("/api/v1/customers/", customer);
            response.EnsureSuccessStatusCode();
            return (customer, response);
        }

        private async Task EnsureCustomerCreated(CustomerDTO customer, HttpResponseMessage prevResponse)
        {
            var response = await client.GetAsync(prevResponse.Headers.Location);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Customer>();
            var expectedCustomer = new Customer(customer) { Id = 1 };
            Assert.That(result, Is.EqualTo(expectedCustomer));
        }

        [Test]
        public async Task Test_CreateCustomer_GivenMissingProperties_ResultsInBadRequest()
        {
            var newCustomer = new CustomerDTO { Name = "Ivan", Surname = "Petrov" };
            var response = await client.PostAsJsonAsync("/api/v1/customers/", newCustomer);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Test_CreateCustomer_GivenDuplicatedEmail_ResultsInBadRequest()
        {
            await PostIvanPetrov();
            await EnsureDuplicatedEmailIsNotPermited();
        }

        private async Task EnsureDuplicatedEmailIsNotPermited()
        {
            var customer2 = new CustomerDTO { Name = "Ivan", Surname = "Sidorov", Email = "ivan@gmail.com" };
            var response = await client.PostAsJsonAsync("/api/v1/customers/", customer2);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Test_UpdateCustomer_GivenDuplicatedEmail_ResultsInBadRequest()
        {
            await PostIvanPetrov();
            var customer = await PostIvanSidorov();
            await EnsureEmailDuplicateIsNotPermitedOnEdit(customer);
        }

        private async Task<CustomerDTO> PostIvanSidorov()
        {
            var customer = new CustomerDTO { Name = "Ivan", Surname = "Sidorov", Email = "ivan.sidorov@gmail.com" };
            var response = await client.PostAsJsonAsync("/api/v1/customers/", customer);
            response.EnsureSuccessStatusCode();
            return customer;
        }

        private async Task EnsureEmailDuplicateIsNotPermitedOnEdit(CustomerDTO customer)
        {
            customer.Email = "ivan@gmail.com";
            var response = await client.PutAsJsonAsync("/api/v1/customers/2/", customer);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Test_DeleteCustomer_IsSuccessful_AndResultsInNotFound()
        {
            await PostIvanPetrov();
            await DeleteIvanPetrov();
            await EnsureDeletedCustomerIsNotFound();
        }

        private async Task DeleteIvanPetrov()
        {
            var response = await client.DeleteAsync("/api/v1/customers/1/");
            response.EnsureSuccessStatusCode();
        }

        private async Task EnsureDeletedCustomerIsNotFound()
        {
            var response = await client.GetAsync("/api/v1/customers/1/");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}