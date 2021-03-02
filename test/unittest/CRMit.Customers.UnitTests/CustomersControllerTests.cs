using CRMit.Customers.Controllers;
using CRMit.Customers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [TearDown]
        public async Task TearDown()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var newCustomer = new Customer { Id = 3 };
            context.Attach(newCustomer);
            context.Remove(newCustomer);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception) { }
        }

        [Test]
        public async Task TestAllCustomersObtained()
        {
            var result = (await customersController.GetListAsync()).Result as JsonResult;
            var list = result.Value as IEnumerable<Customer>;
            CollectionAssert.AreEqual(customers, list);
        }

        [Test]
        public async Task TestCustomerObtained()
        {
            var result = (await customersController.GetCustomerAsync(1)).Result as JsonResult;
            var customer = result.Value as Customer;
            Assert.That(customer.Name, Is.EqualTo("Ivan"));
        }

        [Test]
        public async Task TestIfCustomerIsNotPresent()
        {
            var result = (await customersController.GetCustomerAsync(4)).Result as NotFoundResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task TestOnNegativeId()
        {
            var result = (await customersController.GetCustomerAsync(-1)).Result as BadRequestResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task TestCustomerCreated()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var newCustomer = new Customer { Id = 3, Name = "Petya", Email = "petya.example@gmail.com" };
            
            var result = await customersController.CreateCustomerAsync(newCustomer) as OkResult;

            Assert.That(result, Is.Not.Null);
            var addedCustomer = await context.FindAsync<Customer>(3);
            Assert.That(addedCustomer, Is.EqualTo(newCustomer));
        }

        [Test]
        public async Task TestCustomerCreatedIfIdNotProvided()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var newCustomer = new Customer { Name = "Petya", Email = "petya.example@gmail.com" };
            
            var result = await customersController.CreateCustomerAsync(newCustomer) as OkResult;

            Assert.That(result, Is.Not.Null);
            var addedCustomer = await context.FindAsync<Customer>(3);
            Assert.That(addedCustomer, Is.EqualTo(newCustomer));
        }

        [Test]
        public async Task TestOnAttemptToAddCustomerWithNegativeId()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var newCustomer = new Customer { Id = -1, Name = "Petya", Email = "petya.example@gmail.com" };
            
            var result = await customersController.CreateCustomerAsync(newCustomer) as BadRequestResult;

            Assert.That(result, Is.Not.Null);
        }
    }
}