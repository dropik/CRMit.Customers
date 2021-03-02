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
        public async Task TestGetList()
        {
            var result = (await customersController.GetListAsync()).Result as JsonResult;
            var list = result.Value as IEnumerable<Customer>;
            CollectionAssert.AreEqual(customers, list);
        }

        [Test]
        public async Task TestGetCustomer()
        {
            var result = (await customersController.GetCustomerAsync(1)).Result as JsonResult;
            var customer = result.Value as Customer;
            Assert.That(customer.Name, Is.EqualTo("Ivan"));
        }

        [Test]
        public async Task TestGetCustomerIfNotInDB()
        {
            var result = (await customersController.GetCustomerAsync(4)).Result;
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task TestCreateCustomer()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var newCustomer = new Customer { Id = 3, Name = "Petya", Email = "petya.example@gmail.com" };
            
            var result = await customersController.CreateCustomerAsync(newCustomer);

            Assert.That(result, Is.InstanceOf<OkResult>());
            var addedCustomer = await context.FindAsync<Customer>(3);
            Assert.That(addedCustomer, Is.EqualTo(newCustomer));
        }

        [Test]
        public async Task TestCreateCustomerIfIdNotProvided()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var newCustomer = new Customer { Name = "Petya", Email = "petya.example@gmail.com" };
            
            var result = await customersController.CreateCustomerAsync(newCustomer);

            Assert.That(result, Is.InstanceOf<OkResult>());
            var addedCustomer = await context.FindAsync<Customer>(3);
            Assert.That(addedCustomer, Is.EqualTo(newCustomer));
        }

        [Test]
        public async Task TestCreateCustomerWithNegativeId()
        {
            var newCustomer = new Customer { Id = -1, Name = "Petya", Email = "petya.example@gmail.com" };
            var result = await customersController.CreateCustomerAsync(newCustomer);
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task TestCreateCustomerWithKeyDuplication()
        {
            var newCustomer = new Customer { Id = 2 };
            var result = await customersController.CreateCustomerAsync(newCustomer);
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task TestUpdateCustomer()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var newCustomer = new Customer { Id = 2, Name = "Petya", Email = "petya.example@gmail.com" };

            var result = await customersController.UpdateCustomerAsync(2, newCustomer);
            Assert.That(result, Is.InstanceOf<OkResult>());
            Assert.That(context.Find<Customer>(2), Is.EqualTo(newCustomer));
        }

        [Test]
        public async Task TestUpdateCustomerIfNotExists()
        {
            var newCustomer = new Customer { Id = 3 };
            var result = await customersController.UpdateCustomerAsync(3, newCustomer);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task TestUpdateCustomerOnIdMismatch()
        {
            var newCustomer = new Customer { Id = 2 };
            var result = await customersController.UpdateCustomerAsync(1, newCustomer);
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }
    }
}