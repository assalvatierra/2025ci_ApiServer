using ApiServer.Controllers;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
//using Microsoft.Data.Sqlite;

namespace TestProject1
{
    public class UserControllerTest
    {
        [Fact]
        public void UserController_Test_ShouldReturnWorkingMessage()
        {
            // Arrange
            var controller = new UserController(new SystemUserServices());

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
            var controller = new UserController(new SystemUserServices());
            var request = new UserController.LoginRequest { Username = "testuser", Password = "testpassword" };

            // Act
            var result = await controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            var prop = value?.GetType().GetProperty("token");
            Assert.NotNull(prop);
            var token = prop.GetValue(value) as string;
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public async Task UserController_Login_WithInValidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new UserController(new SystemUserServices());
            var request = new UserController.LoginRequest { Username = "admin", Password = "wrong_password" };

            // Act
            var result = await controller.Login(request);

            // Assert: controller returns UnauthorizedObjectResult for invalid credentials
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var value = unauthorizedResult.Value;
            var prop = value?.GetType().GetProperty("message");
            Assert.NotNull(prop);
            var message = prop.GetValue(value) as string;
            Assert.Equal("Invalid username or password.", message);
        }

        //[Fact]
        //public void SystemUserServices_RegisterUser_ReturnsNewUserId()
        //{
        //    // Arrange
        //    var conn = new SqliteConnection("DataSource=:memory:");
        //    conn.Open();

        //    // create AspNetUsers table (columns used by service)
        //    using var create = conn.CreateCommand();
        //    create.CommandText = @"
        //    CREATE TABLE AspNetUsers (
        //      Id TEXT PRIMARY KEY,
        //      UserName TEXT,
        //      NormalizedUserName TEXT,
        //      Email TEXT,
        //      NormalizedEmail TEXT,
        //      PhoneNumber TEXT,
        //      PasswordHash TEXT,
        //      EmailConfirmed INTEGER,
        //      PhoneNumberConfirmed INTEGER,
        //      TwoFactorEnabled INTEGER,
        //      LockoutEnabled INTEGER,
        //      AccessFailedCount INTEGER
        //    );";
        //    create.ExecuteNonQuery();

        //    // connection factory for the service
        //    Func<DbConnection> factory = () => {
        //        // return a new connection instance referencing same in-memory DB
        //        var c = new SqliteConnection("DataSource=:memory:;Cache=Shared");
        //        c.Open();
        //        return c;
        //    };

        //    // ensure the service uses the same shared memory DB: open primary connection first, then factory uses Cache=Shared
        //    conn.Close();
        //    conn = new SqliteConnection("DataSource=:memory:;Cache=Shared");
        //    conn.Open();
        //    // run create SQL again on this shared connection or use the same pattern above

        //    var systemUserService = new SystemUserServices(factory);

        //    // Act
        //    var result = systemUserService.registerUser("testuser", "testpassword", "e@x.com", "123");

        //    // Assert
        //    Assert.True(result > 0, "registerUser should return a positive user id on success.");
        //}
    }
}
