using Microsoft.Extensions.Configuration;
using Xunit;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.Equal(1, 1);
        }


        [Fact]
        public void ApiServer_test_ShouldReturnExpectedString()
        {
            // Arrange
            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddInMemoryCollection(new System.Collections.Generic.Dictionary<string, string>
                {
                    {"ConnectionStrings:DefaultConnection", "Host=localhost;Username=test;Password=test;Database=testdb"}
                })
                .Build();
            var controller = new ApiServer.Controllers.DatabaseController(configuration);
            // Act
            var result = controller.test();
            // Assert
            Assert.Equal("Reply from test-api-call endpoint.", result);
        }


    }
}