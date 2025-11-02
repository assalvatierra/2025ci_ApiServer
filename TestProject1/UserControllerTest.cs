using ApiServer.Controllers;
using ApiServer.Services;
using ApiServer.Postgres.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using System.Collections;

namespace TestProject1
{
    public class UserControllerTest
    {
        [Fact]
        public void UserController_Test_ShouldReturnWorkingMessage()
        {
            // Arrange
            var mockRepo = new Mock<IAspNetUserRepo>();
            var systemUserServices = new SystemUserServices(mockRepo.Object);
            var controller = new UserController(systemUserServices);

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
            var username = "testuser";
            var plainPassword = "testpassword";

            // Prepare a mocked repo that returns a user with a valid hashed password
            var mockRepo = new Mock<IAspNetUserRepo>();
            var hasher = new PasswordHasher<object>();
            var hashed = hasher.HashPassword(null, plainPassword);

            mockRepo
                .Setup(r => r.GetUserByUsernameAsync(username))
                .ReturnsAsync(new AspNetUser { UserName = username, PasswordHash = hashed });

            var systemUserServices = new SystemUserServices(mockRepo.Object);
            var controller = new UserController(systemUserServices);
            var request = new UserController.LoginRequest { Username = username, Password = plainPassword };

            // Act
            var result = await controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            var tokenProp = value?.GetType().GetProperty("token");
            Assert.NotNull(tokenProp);
            var token = tokenProp.GetValue(value) as string;
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public async Task UserController_Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var username = "admin";
            var wrongPassword = "wrong_password";

            // Mock repo to return null (user not found) or a user with different password
            var mockRepo = new Mock<IAspNetUserRepo>();
            mockRepo
                .Setup(r => r.GetUserByUsernameAsync(username))
                .ReturnsAsync((AspNetUser?)null);

            var systemUserServices = new SystemUserServices(mockRepo.Object);
            var controller = new UserController(systemUserServices);
            var request = new UserController.LoginRequest { Username = username, Password = wrongPassword };

            // Act
            var result = await controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var value = unauthorizedResult.Value;
            var prop = value?.GetType().GetProperty("message");
            Assert.NotNull(prop);
            var message = prop.GetValue(value) as string;
            Assert.Equal("Invalid username or password.", message);
        }
    }



    public class CalculatorTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 1, 2, 3 };
            yield return new object[] { -4, -6, -10 };
            yield return new object[] { -2, 2, 0 };
            yield return new object[] { int.MinValue, -1, int.MaxValue };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


}
