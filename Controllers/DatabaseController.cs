using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DatabaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet(Name = "test-api-call")]
        public string test()
        {
            return "hi";
        }

        [HttpPost(Name = "initialize")]
        public void initialize()
        {
            string initializationScript = @"
                CREATE TABLE IF NOT EXISTS sample_table (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(100) NOT NULL
                );
                INSERT INTO sample_table (name) VALUES ('Sample Data 1'), ('Sample Data 2');
            ";

            initializeDatabase(initializationScript);
        }


        public void initializeDatabase(string initializationScript)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(initializationScript, connection);
            command.ExecuteNonQuery();
        }
    }
}
