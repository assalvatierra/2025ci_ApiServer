using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiServer.Services
{
    public class SystemUserServices
    {
        public int LoginWithUsernamePassword(string username, string password)
        {
            // Placeholder logic for user authentication
            if (username == "admin" && password == "password")
            {
                return 1; // Simulated user ID
            }
            return -1; // Authentication failed
        }

        public string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("123456-123456-123456-123456-123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "ABC",
                audience: "ALL",
                claims: new List<Claim>(new[] { new Claim("role", "admin") }),
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
