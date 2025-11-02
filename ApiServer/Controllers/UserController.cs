using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
//using System.Threading.Tasks;
using ApiServer.Postgres;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly SystemUserServices _UserServices;

        public UserController(SystemUserServices userServices)
        {
            this._UserServices = userServices;
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

        public class RegisterRequest
        {
            [Required]
            public string? Username { get; set; }

            [Required]
            public string? Password { get; set; }

            public string? Email { get; set; }

            public string? PhoneNumber { get; set; }
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

            // Login with username and password using PostgreSQLService
            int IsLoginSuccessful = await this._UserServices.LoginWithUsernamePassword(username, password);

            // Generate Jwt Token if login is successful
            if (IsLoginSuccessful <= 0)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Generate Jwt Token if login is successful
            var token = this._UserServices.GenerateJwtToken(username);

            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error generating token." });
            }

            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = request.Username;
            var password = request.Password;
            var email = request.Email;
            var phone = request.PhoneNumber;

            if (username == null || password == null)
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var result = await this._UserServices.registerUser(username, password, email, phone);
            if (result <= 0)
            {
                return BadRequest(new { message = "Registration failed." });
            }

            return Ok(new { message = "Registered successfully" });
        }


    }
}
