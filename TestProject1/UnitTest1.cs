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

        //[Fact]
        //public void ApiServer_initializeDatabase_ShouldCreateTableAndInsertData()
        //{
        //    // Arrange
        //    var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
        //        .AddInMemoryCollection(new System.Collections.Generic.Dictionary<string, string>
        //        {
        //            {"ConnectionStrings:DefaultConnection", "Host=localhost;Username=test;Password=test;Database=testdb"}
        //        })
        //        .Build();
        //    var controller = new ApiServer.Controllers.DatabaseController(configuration);
        //    string initializationScript = @"
        //        CREATE TABLE IF NOT EXISTS sample_table (
        //            id SERIAL PRIMARY KEY,
        //            name VARCHAR(100) NOT NULL
        //        );
        //        INSERT INTO sample_table (name) VALUES ('Sample Data 1'), ('Sample Data 2');
        //    ";
        //    // Act
        //    controller.initializeDatabase(initializationScript);
        //    // Assert
        //    using var connection = new Npgsql.NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        //    connection.Open();
        //    using var command = new Npgsql.NpgsqlCommand("SELECT COUNT(*) FROM sample_table;", connection);
        //    var count = (long)command.ExecuteScalar();
        //    Assert.Equal(2, count);
        //}


    }
}