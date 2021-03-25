using CRMit.Customers.Models;
using CRMit.Customers.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        private HttpClient client;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var server = CreateTestServer();
            await ExecuteStartupTasks(server);
            client = server.CreateClient();
        }

        private static TestServer CreateTestServer() => new(new WebHostBuilder()
            .UseEnvironment("Development")
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("appsettings.json", optional: true)
                       .AddJsonFile("appsettings.Development.json", optional: true);
            })
            .UseStartup<Startup>());

        private static async Task ExecuteStartupTasks(TestServer server)
        {
            var tasks = server.Services.GetServices<IStartupTask>();
            foreach (var task in tasks)
            {
                await task.ExecuteAsync();
            }
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

        private async Task<(CustomerInput customer, HttpResponseMessage response)> PostIvanPetrov()
        {
            var customer = new CustomerInput { Name = "Ivan", Surname = "Petrov", Email = "ivan@gmail.com" };
            var response = await client.PostAsJsonAsync("/crmit/v1/customers/", customer);
            response.EnsureSuccessStatusCode();
            return (customer, response);
        }

        private async Task EnsureCustomerCreated(CustomerInput customer, HttpResponseMessage prevResponse)
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
            var newCustomer = new CustomerInput { Name = "Ivan", Surname = "Petrov" };
            var response = await client.PostAsJsonAsync("/crmit/v1/customers/", newCustomer);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Test_CreateCustomer_GivenBadEmail_ResultsInBadRequest()
        {
            var newCustomer = new CustomerInput { Name = "Ivan", Surname = "Petrov", Email = "ivan.petrov" };
            var response = await client.PostAsJsonAsync("/crmit/v1/customers/", newCustomer);
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
            var customer2 = new CustomerInput { Name = "Ivan", Surname = "Sidorov", Email = "ivan@gmail.com" };
            var response = await client.PostAsJsonAsync("/crmit/v1/customers/", customer2);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Test_UpdateCustomer_GivenDuplicatedEmail_ResultsInBadRequest()
        {
            await PostIvanPetrov();
            var customer = await PostIvanSidorov();
            await EnsureEmailDuplicateIsNotPermitedOnEdit(customer);
        }

        private async Task<CustomerInput> PostIvanSidorov()
        {
            var customer = new CustomerInput { Name = "Ivan", Surname = "Sidorov", Email = "ivan.sidorov@gmail.com" };
            var response = await client.PostAsJsonAsync("/crmit/v1/customers/", customer);
            response.EnsureSuccessStatusCode();
            return customer;
        }

        private async Task EnsureEmailDuplicateIsNotPermitedOnEdit(CustomerInput customer)
        {
            customer.Email = "ivan@gmail.com";
            var response = await client.PutAsJsonAsync("/crmit/v1/customers/2/", customer);
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
            var response = await client.DeleteAsync("/crmit/v1/customers/1/");
            response.EnsureSuccessStatusCode();
        }

        private async Task EnsureDeletedCustomerIsNotFound()
        {
            var response = await client.GetAsync("/crmit/v1/customers/1/");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}