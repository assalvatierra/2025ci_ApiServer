using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class loginController: ControllerBase
    {
        private readonly IConfiguration _configuration;

        public loginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet(Name = "test-login-api-call")]
        public string Login(string username, string password)
        {
            return "Reply from test-login-api-call endpoint.";
        }
    }
}
