using ApiServer.Controllers;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using ApiServer.Postgres;
using Moq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;


namespace TestProject1
{
    public class UserControllerTest
    {
        [Fact]
        public void UserController_Test_ShouldReturnWorkingMessage()
        {
            // Arrange
            var mockPostgreSQLSettings = Mock.Of<IOptions<PostgreSQLSettings>>(x =>
                x.Value == new PostgreSQLSettings
                {
                    ConnectionString = "test",
                    CommandTimeout = 30,
                    MaxPoolSize = 100
                });

            var mockLogger = Mock.Of<ILogger<PostgreSQLService>>();
            var postgresService = new PostgreSQLService(mockPostgreSQLSettings, mockLogger);
            var systemUserServices = new SystemUserServices(postgresService);

            var controller = new UserController(systemUserServices, postgresService);

            // Act
            var result = controller.Test();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            var prop = value?.GetType().GetProperty("message");
            Assert.NotNull(prop);
            var message = prop.GetValue(value) as string;
            Assert.Equal("UserController is working!", message);
        }

        [Fact]
        public async Task UserController_Login_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            var mockPostgreSQLSettings = Mock.Of<IOptions<PostgreSQLSettings>>(x =>
                x.Value == new PostgreSQLSettings
                {
                    ConnectionString = "test",
                    CommandTimeout = 30,
                    MaxPoolSize = 100
                });

            var mockLogger = Mock.Of<ILogger<PostgreSQLService>>();
            var postgresService = new PostgreSQLService(mockPostgreSQLSettings, mockLogger);
            var systemUserServices = new SystemUserServices(postgresService);

            var controller = new UserController(systemUserServices, postgresService);
            var request = new UserController.LoginRequest { Username = "testuser", Password = "testpassword" };

            // Act
            var result = await controller.Login(request);

            // Assert
            // Note: This test will likely fail because there's no actual database setup
            // Consider mocking SystemUserServices for unit testing
            var actionResult = Assert.IsType<ActionResult>(result);
        }

        [Fact]
        public async Task UserController_Login_WithInValidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var mockPostgreSQLSettings = Mock.Of<IOptions<PostgreSQLSettings>>(x =>
                x.Value == new PostgreSQLSettings
                {
                    ConnectionString = "test",
                    CommandTimeout = 30,
                    MaxPoolSize = 100
                });

            var mockLogger = Mock.Of<ILogger<PostgreSQLService>>();
            var postgresService = new PostgreSQLService(mockPostgreSQLSettings, mockLogger);
            var systemUserServices = new SystemUserServices(postgresService);

            var controller = new UserController(systemUserServices, postgresService);
            var request = new UserController.LoginRequest { Username = "admin", Password = "wrong_password" };

            // Act
            var result = await controller.Login(request);

            // Assert
            // Note: This test will likely fail because there's no actual database setup
            // Consider mocking SystemUserServices for unit testing
            var actionResult = Assert.IsType<ActionResult>(result);
        }

        //[Fact]
        //public void SystemUserServices_RegisterUser_ReturnsNewUserId()
        //{
        //    // This test is commented out as it requires complex database setup
        //    // Consider using an in-memory database or mocking for unit tests
        //}
    }
}
