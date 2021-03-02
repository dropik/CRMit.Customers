using CRMit.Customers.Controllers;
using CRMit.Customers.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace CRMit.Customers
{
    [TestFixture]
    public class CustomersControllerTests
    {
        private readonly List<Customer> customers = new List<Customer>()
        {
            new Customer { Id = 1, Name = "Ivan", Email = "ivan.example@gmail.com" },
            new Customer { Id = 2, Name = "Vasia", Email = "vasia.example@gmail.com" }
        };

        private DbContextOptions<CustomersDbContext> dbContextOptions = new DbContextOptionsBuilder<CustomersDbContext>()
            .UseInMemoryDatabase(databaseName: "CustomersDB")
            .Options;
        private CustomersController customersController;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            await context.AddRangeAsync(customers);
            await context.SaveChangesAsync();
        }

        [SetUp]
        public void Setup()
        {
            customersController = new CustomersController(new CustomersDbContext(dbContextOptions));
        }

        [Test]
        public async Task TestAllCustomersObtained()
        {
            var result = await customersController.ListAsync() as JsonResult<IEnumerable<Customer>>;
            CollectionAssert.AreEqual(customers, result.Content);
        }

        [Test]
        public async Task TestCustomerObtained()
        {
            var result = await customersController.GetCustomerAsync(1) as JsonResult<Customer>;
            Assert.That(result.Content.Name, Is.EqualTo("Ivan"));
        }

        [Test]
        public async Task TestIfCustomerIsNotPresent()
        {
            var result = await customersController.GetCustomerAsync(4) as NotFoundResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task TestOnNegativeId()
        {
            var result = await customersController.GetCustomerAsync(-1) as BadRequestResult;
            Assert.That(result, Is.Not.Null);
        }
    }
}