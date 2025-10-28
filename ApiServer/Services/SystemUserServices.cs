using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace ApiServer.Services
{
    public class SystemUserServices
    {
        private readonly IConfiguration _configuration;

        // Parameterless constructor for tests and backward compatibility
        public SystemUserServices()
            : this(new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: false).AddEnvironmentVariables().Build())
        {
        }

        public SystemUserServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int LoginWithUsernamePassword(string username, string password)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return -1;

            try
            {
                var user = getUserbyUsername(username);
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

        public int registerUser(string username, string password, string? email = null, string? phonenumber = null)
        {
            // Prevent duplicate usernames
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return -1;

                var existing = getUserbyUsername(username);
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
                var connStr = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connStr))
                    return -1;

                using var connection = new NpgsqlConnection(connStr);
                connection.Open();

                // Use ASP.NET Core Identity PasswordHasher to produce a compatible PasswordHash
                var hasher = new PasswordHasher<object>();
                var passwordHash = hasher.HashPassword(null, password);

                var id = Guid.NewGuid().ToString();
                var normalized = username?.ToUpperInvariant() ?? string.Empty;
                var normalizedEmail = email?.ToUpperInvariant();
                var securityStamp = Guid.NewGuid().ToString();
                var concurrencyStamp = Guid.NewGuid().ToString();

                var insertSql = @"
                    INSERT INTO ""AspNetUsers"" (
                        ""Id"", ""UserName"", ""NormalizedUserName"",
                        ""Email"", ""NormalizedEmail"", ""PhoneNumber"", 
                        ""PasswordHash"", 
                        ""EmailConfirmed"", ""PhoneNumberConfirmed"", ""TwoFactorEnabled"", ""LockoutEnabled"", ""AccessFailedCount""
                    )
                    VALUES (
                        @Id, @UserName, @NormalizedUserName,
                        @Email, @NormalizedEmail, @PhoneNumber,
                        @PasswordHash, 
                        false, false, false, false, 0
                    )";

                using var cmd = new NpgsqlCommand(insertSql, connection);
                cmd.Parameters.AddWithValue("Id", id);
                cmd.Parameters.AddWithValue("UserName", username ?? string.Empty);
                cmd.Parameters.AddWithValue("NormalizedUserName", normalized);
                cmd.Parameters.AddWithValue("Email", (object?)email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("NormalizedEmail", (object?)normalizedEmail ?? DBNull.Value);
                cmd.Parameters.AddWithValue("PhoneNumber", (object?)phonenumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("PasswordHash", passwordHash ?? string.Empty);

                var rows = cmd.ExecuteNonQuery();
                return rows > 0 ? 1 : -1;
            }
            catch
            {
                return -1;
            }
        }

        public SystemUser? getUserbyUsername(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    return null;

                var connStr = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connStr))
                    return null;

                using var connection = new NpgsqlConnection(connStr);
                connection.Open();

                var sql = @"
                    SELECT ""Id"", ""UserName"", ""Email"", ""PhoneNumber"", ""PasswordHash""
                    FROM ""AspNetUsers""
                    WHERE ""UserName"" = @UserName OR ""NormalizedUserName"" = @NormalizedUserName
                    LIMIT 1";

                using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@UserName", username);
                cmd.Parameters.AddWithValue("@NormalizedUserName", username.ToUpperInvariant());

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return null; // user not found

                var user = new SystemUser
                {
                    Id = reader["Id"] == DBNull.Value ? null : reader["Id"].ToString(),
                    UserName = reader["UserName"] == DBNull.Value ? null : reader["UserName"].ToString(),
                    Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                    PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : reader["PhoneNumber"].ToString(),
                    PasswordHash = reader["PasswordHash"] == DBNull.Value ? null : reader["PasswordHash"].ToString()
                };

                return user;
            }
            catch
            {
                return null;
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
