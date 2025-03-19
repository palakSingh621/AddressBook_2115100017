using System.Security.Claims;
using AddressBook_App.Controllers;
using BusinessLayer.Service;
using CacheLayer.Interface;
using CacheLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Service;
using StackExchange.Redis;

namespace TestingLayer.Tests
{
    public class AddressBookControllerTests
    {
        private AddressBook_AppController _controller;
        private AddressBookContext _context;
        private AddressBookRepository _repository;
        private AddressBookService _service;
        private IRedisCacheService _redisCacheService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            _context = new AddressBookContext(options);

            _repository = new AddressBookRepository(_context);
            _service = new AddressBookService(_repository);

            var redisMock = ConnectionMultiplexer.Connect("localhost:6379");
            _redisCacheService = new RedisCacheService(redisMock);

            var logger = new LoggerFactory().CreateLogger<AddressBook_AppController>();
            _controller = new AddressBook_AppController(logger, _service, _redisCacheService);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", "2")
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            //_context.Users.Add(new UserEntity
            //{
            //    Id = 1,
            //    UserName = "TestUser",
            //    Email = "test@mail.com",
            //    PasswordHash = "abc",
            //    Role = "User"
            //});

            _context.AddressBookContacts.Add(new AddressBookEntity
            {
                Id = 1,
                UserId = 2,
                ContactName = "John",
                ContactNumber = "123456",
                Email = "john@mail.com",
                Address = "TestAddress"
            });
            _context.SaveChanges();
        }
        [Test]
        public async Task GetAllContactsTest()
        {
            var result = await _controller.GetAllContacts() as ObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetContactByIdTest()
        {
            var result = await _controller.GetContactById(1) as ObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void AddContactTest()
        {
            var result = _controller.AddContact("Jane", "9876543210", "jane@mail.com", "New Address") as ObjectResult;
            Console.WriteLine(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task UpdateContactByIdTest()
        {
            var result = await _controller.UpdateContactById(1, "UpdatedName", "0000000000", "updated@mail.com", "Updated Address") as ObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task DeleteContactByIdTest()
        {
            var result = await _controller.DeleteContactById(1) as OkObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    }
}
