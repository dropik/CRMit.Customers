using CRMit.Customers.Controllers;
using CRMit.Customers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMit.Customers
{
    [TestFixture]
    public class CustomersControllerTests
    {
        private readonly List<Customer> customers = new List<Customer>()
        {
            new Customer { Id = 1, Name = "Ivan", Surname = "Petrov", Email = "ivan.example@gmail.com" },
            new Customer { Id = 2, Name = "Vasia", Surname = "Pupkin", Email = "vasia.example@gmail.com" }
        };
        private readonly DbContextOptions<CustomersDbContext> dbContextOptions = new DbContextOptionsBuilder<CustomersDbContext>()
            .UseInMemoryDatabase(databaseName: "CustomersDB")
            .Options;

        private CustomersController customersController;

        [SetUp]
        public async Task Setup()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            await context.AddRangeAsync(customers);
            await context.SaveChangesAsync();
            customersController = new CustomersController(new CustomersDbContext(dbContextOptions));
        }

        [TearDown]
        public async Task TearDown()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            await context.Database.EnsureDeletedAsync();
        }

        [Test]
        public async Task TestCreateCustomer()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var newCustomerDTO = new CustomerDTO { Name = "Petya", Surname = "Ivanov", Email = "petya.example@gmail.com" };
            var newCustomer = new Customer(newCustomerDTO) { Id = 3 };

            var result = (await customersController.CreateCustomerAsync(newCustomerDTO)).Result as CreatedAtActionResult;
            Assert.That(result.ActionName, Is.EqualTo("GetCustomerAsync"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(3));
            Assert.That(result.Value, Is.EqualTo(newCustomer));
            var addedCustomer = await context.FindAsync<Customer>((long)3);
            Assert.That(addedCustomer, Is.EqualTo(newCustomer));
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
        public async Task TestUpdateCustomer()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var customerDTO = new CustomerDTO { Name = "Petya", Surname = "Ivanov", Email = "petya.example@gmail.com" };
            var customer = new Customer(customerDTO) { Id = 2 };

            var result = await customersController.UpdateCustomerAsync(2, customerDTO);
            Assert.That(result, Is.InstanceOf<OkResult>());
            Assert.That(context.Find<Customer>((long)2), Is.EqualTo(customer));
        }

        [Test]
        public async Task TestUpdateCustomerIfNotExists()
        {
            var customer = new CustomerDTO();
            var result = await customersController.UpdateCustomerAsync(3, customer);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task TestDeleteCustomer()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var id = 2;

            var result = await customersController.DeleteCustomerAsync(id);

            Assert.That(result, Is.InstanceOf<OkResult>());
            Assert.That(context.Customers.Any(c => c.Id == id), Is.False);
        }

        [Test]
        public async Task TestDeleteCustomerOnMissingCustomer()
        {
            using var context = new CustomersDbContext(dbContextOptions);
            var result = await customersController.DeleteCustomerAsync(3);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            Assert.That(context.Customers.Count(), Is.EqualTo(2));
        }
    }
}