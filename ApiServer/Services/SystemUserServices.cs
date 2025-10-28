using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using ApiServer.Postgres;
using ApiServer.Postgres.Repository;

namespace ApiServer.Services
{
    public class SystemUserServices
    {

        //private readonly PostgreSQLService _postgresService;
        private IAspNetUserRepo _repo;

        public SystemUserServices(IAspNetUserRepo repo)
        {
            this._repo = repo;
        }

        public async Task<int> LoginWithUsernamePassword(string username, string password)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return -1;

            try
            {
                var user = await this._repo.GetUserByUsernameAsync(username);
                if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                    return -1;

                var hasher = new PasswordHasher<object>();
                var verify = hasher.VerifyHashedPassword(null, user.PasswordHash, password);

                return verify == PasswordVerificationResult.Success || verify == PasswordVerificationResult.SuccessRehashNeeded ? 1 : -1;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> registerUser(string username, string password, string? email = null, string? phonenumber = null)
        {
            // Prevent duplicate usernames
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return -1;

                var existing = await this._repo.GetUserByUsernameAsync(username);
                if (existing != null)
                    return -1; // username already exists
            }
            catch
            {
                // If check fails, continue to attempt registration (or return -1). We'll return -1 to be safe.
                return -1;
            }

            // Save user into AspNetUsers table with hashed password
            try
            {

                // Use ASP.NET Core Identity PasswordHasher to produce a compatible PasswordHash
                var hasher = new PasswordHasher<object>();
                var passwordHash = hasher.HashPassword(null, password);

                int rows = await this._repo.CreateUserAsync(username, email ?? string.Empty, passwordHash, phonenumber);

                return rows > 0 ? 1 : -1;
            }
            catch
            {
                return -1;
            }
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

    public class SystemUser
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PasswordHash { get; set; } = null;

    }
}
