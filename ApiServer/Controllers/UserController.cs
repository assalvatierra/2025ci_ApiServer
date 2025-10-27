using ApiServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SystemUserServices _UserServices;

        public UserController(SystemUserServices _UserServices)
        {
            this._UserServices = _UserServices;
        }

        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok(new { message = "UserController is working!" });
        }

        public class LoginRequest
        {
            [Required]
            public string? Username { get; set; }

            [Required]
            public string? Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Parse request data
            var username = request.Username;
            var password = request.Password;
            if (username == null || password == null)
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            // Login with username and password
            int IsLoginSuccessful = this._UserServices.LoginWithUsernamePassword(username, password);

            // Generate Jwt Token if login is successful
            if (IsLoginSuccessful <= 0)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Generate Jwt Token if login is successful
            var token = this._UserServices.GenerateJwtToken(username);

            if (token == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error generating token." });
            }

            return Ok(new { token });
        }


    }
}
