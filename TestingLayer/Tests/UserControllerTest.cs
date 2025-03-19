using System.Security.Claims;
using AddressBook_App.Controllers;
using BusinessLayer.Service;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.Model;
using NUnit.Framework;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using RepositoryLayer.Context;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Helper;
using Middleware.RabbitMQ.Interface;
using Moq;
using Microsoft.Extensions.Configuration;

namespace TestingLayer.Tests
{
    [TestFixture]
    public class UserControllerTest
    {
        [TestFixture]
        public class UserControllerTests
        {
            private UserController _controller;
            private IUserBL _userBL;
            private ILogger<UserController> _logger;
            private Mock<IRabbitMQProducer> _mockRabbitMQProducer;
            private AddressBookContext _context;

            [SetUp]
            public void Setup()
            {
                var options = new DbContextOptionsBuilder<AddressBookContext>()
                    .UseInMemoryDatabase(databaseName: "TestAddressBookDb")
                    .Options;

                _context = new AddressBookContext(options);
                IUserRL userRL = new UserRL(_context);
                var inMemorySettings = new Dictionary<string, string> {
                    {"Jwt:Key", "e72a99690124276d58943ba3bf9866a5ea55ec36eb8e4732cf29d192fde7d6ad803f95523391c63781ff996302ff9bd7e691f27a6e8491d4db25e98490d35b714554f690fdbb5a920a16421f078544dc44d878d14b194f9024dd97a00dbf01f07ec00c22be852ec284c44776c032181611d764fc767a5f9cb4d805e051d56e7c77feb97897d64c20caaba183d6d2dfff95de3aac7af9beea005ca2bc43077bbabb86c4a347308984f399a1f48890706fa53f50ce12eb7636b7a331b8e1e7240532aebc6e371a6cc1f6c80f6ee4acbcee5173a8b7bd22b21b0d9ee6767ff4a23f6fabebea642df8e7441095348bcbc0fdbb5ad01330d872e99f4f8e38f5e2cd85"},
                    {"Jwt:Issuer", "testIssuer"},
                    {"Jwt:Audience", "testAudience"},
                    {"Jwt:ResetSecret", "e72a99690124276d58943ba3bf9866a5ea55ec36eb8e4732cf29d192fde7d6ad803f95523391c63781ff996302ff9bd7e691f27a6e8491d4db25e98490d35b714554f690fdbb5a920a16421f078544dc44d878d14b194f9024dd97a00dbf01f07ec00c22be852ec284c44776c032181611d764fc767a5f9cb4d805e051d56e7c77feb97897d64c20caaba183d6d2dfff95de3aac7af9beea005ca2bc43077bbabb86c4a347308984f399a1f48890706fa53f50ce12eb7636b7a331b8e1e7240532aebc6e371a6cc1f6c80f6ee4acbcee5173a8b7bd22b21b0d9ee6767ff4a23f6fabebea642df8e7441095348bcbc0fdbb5ad01330d872e99f4f8e38f5e2cd85" }
                };

                IConfiguration configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(inMemorySettings)
                    .Build();

                JwtTokenHelper jwtTokenHelper = new JwtTokenHelper(configuration);
                _userBL = new UserBL(userRL, jwtTokenHelper);

                _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<UserController>();
                _mockRabbitMQProducer = new Mock<IRabbitMQProducer>();
                _controller = new UserController(_logger, _userBL, _mockRabbitMQProducer.Object);

            }

            [Test]
            public void Register_ReturnsOk_WhenSuccessful()
            {
                var model = new RegisterRequest
                {
                    UserName = "TestUser",
                    Email = "test@example.com",
                    Password = "password123"
                };

                var result = _controller.Register(model) as OkObjectResult;

                Assert.That(result.StatusCode, Is.EqualTo(200));
            }

            [Test]
            public void Login_ReturnsOk_WhenSuccessful()
            {
                var model = new LoginRequest
                {
                    Email = "test@example.com",
                    Password = "password123"
                };

                var registerModel = new RegisterRequest
                {
                    UserName = "TestUser",
                    Email = model.Email,
                    Password = model.Password
                };
                _controller.Register(registerModel);

                var result = _controller.Login(model) as OkObjectResult;

                Assert.That(result.StatusCode, Is.EqualTo(200));
            }

            [Test]
            public async Task ForgotPassword_ReturnsOk_WhenEmailIsValid()
            {
                var model = new RegisterRequest
                {
                    UserName = "TestUser",
                    Email = "palak.singh_cs.ccv21@gla.ac.in",
                    Password = "password123",
                    Role="User"
                };
                _controller.Register(model);

                var forgetModel = new ForgetPasswordRequest { Email = model.Email };
                var result = await _controller.ForgotPassword(forgetModel) as OkObjectResult;

                Assert.That(result.StatusCode, Is.EqualTo(200));
            }
            [Test]
            public void GetUserProfile_ReturnsOk_WhenUserIdExists()
            {
                var registerModel = new RegisterRequest
                {
                    UserName = "ProfileUser",
                    Email = "profile@example.com",
                    Password = "password123"
                };
                _controller.Register(registerModel);

                var userId = _context.Users.FirstOrDefault(u => u.Email == registerModel.Email)?.Id;
                var identity = new ClaimsIdentity(new[] { new Claim("UserId", userId.ToString()) });
                var user = new ClaimsPrincipal(identity);
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                };

                var result = _controller.GetUserProfile() as OkObjectResult;

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

}

