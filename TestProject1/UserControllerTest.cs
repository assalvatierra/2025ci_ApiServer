using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ApiServer.Controllers;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;

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
            var request = new UserController.LoginRequest { Username = "admin", Password = "password" };

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
    }
}
