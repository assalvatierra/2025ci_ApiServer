using ApiServer.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace ApiServer.Postgres.Repository
{
    public class AspNetUserRepo : IAspNetUserRepo
    {
        private readonly PostgreSQLService _postgresService;

        public AspNetUserRepo(PostgreSQLService postgresService)
        {
            _postgresService = postgresService;
        }

        public async Task<int> CreateUserAsync(string username, string email, string passwordHash, string? phoneNumber = null)
        {
            var sql = "INSERT INTO public.\"AspNetUsers\"\r\n(\"Id\", \"UserName\", \"NormalizedUserName\", \"Email\", \"NormalizedEmail\", \"EmailConfirmed\", \"PasswordHash\", \"PhoneNumber\", \"PhoneNumberConfirmed\", \"TwoFactorEnabled\", \"LockoutEnabled\", \"AccessFailedCount\")\r\n"
                +"VALUES(@Id, @UserName, '', @Email, '', false, @PasswordHash, @PhoneNumber, false, false, false, 0);";

            var parameters = new[]
            {
                new NpgsqlParameter("@Id", Guid.NewGuid().ToString()),
                new NpgsqlParameter("@UserName", username),
                new NpgsqlParameter("@Email", email),
                new NpgsqlParameter("@PasswordHash", passwordHash),
                new NpgsqlParameter("@PhoneNumber", (object?)phoneNumber ?? DBNull.Value),
            };

            try { 
                Console.WriteLine("Executing SQL: " + sql);
                return await _postgresService.ExecuteNonQueryAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error logging SQL: " + ex.Message);
                return -1;
            }
            
        }

        public async Task<AspNetUser?> GetUserByIdAsync(string userId)
        {
            var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id AND IsActive = true";
            var parameters = new[]
            {
                new NpgsqlParameter("@Id", userId)
            };

            var users = await _postgresService.ExecuteQueryAsync(
                sql,
                reader => new AspNetUser
                {
                    Id = reader.GetString(reader.GetOrdinal("Id")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    LastModifiedDate = reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastModifiedDate"))
                },
                parameters
            );
            //var users = await _postgresService.ExecuteQueryAsync<AspNetUser>(sql, parameters);
            return users;
        }

        public async Task<AspNetUser?> GetUserByUsernameAsync(string username)
        {
            var sql = "SELECT * FROM public.\"AspNetUsers\" WHERE \"UserName\" = @UserName";

            var parameters = new[]
            {
                new NpgsqlParameter("@UserName", username)
            };
            try
            {
                var users = await _postgresService.ExecuteQueryAsync<AspNetUser>(sql,
                    reader => new AspNetUser
                    {
                        Id = reader.GetString(reader.GetOrdinal("Id")),
                        UserName = reader.GetString(reader.GetOrdinal("UserName")),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                        PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed"))
                    },
                    parameters);
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserByUsernameAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<AspNetUser?> GetUserByEmailAsync(string email)
        {
            var sql = "SELECT * FROM AspNetUsers WHERE Email = @Email AND IsActive = true";
            var parameters = new[]
            {
                new NpgsqlParameter("@Email", email)
            };

            var users = await _postgresService.ExecuteQueryAsync<AspNetUser>(sql,
                reader => new AspNetUser
                {
                    Id = reader.GetString(reader.GetOrdinal("Id")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    LastModifiedDate = reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastModifiedDate"))
                },
                parameters);
            return users;
        }

        public async Task<IEnumerable<AspNetUser>> GetAllUsersAsync()
        {
            var sql = "SELECT * FROM AspNetUsers WHERE IsActive = true ORDER BY CreatedDate DESC";
            var users = new List<AspNetUser>();

            await using var connection = await _postgresService.GetConnectionAsync();
            await using var command = new NpgsqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new AspNetUser
                {
                    Id = reader.GetString(reader.GetOrdinal("Id")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    LastModifiedDate = reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastModifiedDate"))
                });
            }

            return users;
        }

        public async Task<bool> UpdateUserAsync(AspNetUser user)
        {
            var sql = @"UPDATE AspNetUsers SET 
                       UserName = @UserName, Email = @Email, PhoneNumber = @PhoneNumber, 
                       EmailConfirmed = @EmailConfirmed, LastModifiedDate = @LastModifiedDate 
                       WHERE Id = @Id";

            var parameters = new[]
            {
                new NpgsqlParameter("@UserName", user.UserName),
                new NpgsqlParameter("@Email", user.Email),
                new NpgsqlParameter("@PhoneNumber", (object?)user.PhoneNumber ?? DBNull.Value),
                new NpgsqlParameter("@EmailConfirmed", user.EmailConfirmed),
                new NpgsqlParameter("@LastModifiedDate", DateTime.UtcNow),
                new NpgsqlParameter("@Id", user.Id)
            };

            var result = await _postgresService.ExecuteNonQueryAsync(sql, parameters);
            return result > 0;
        }

        public async Task<bool> UpdatePasswordHashAsync(string userId, string newPasswordHash)
        {
            var sql = "UPDATE AspNetUsers SET PasswordHash = @PasswordHash, LastModifiedDate = @LastModifiedDate WHERE Id = @Id";
            var parameters = new[]
            {
                new NpgsqlParameter("@PasswordHash", newPasswordHash),
                new NpgsqlParameter("@LastModifiedDate", DateTime.UtcNow),
                new NpgsqlParameter("@Id", userId)
            };

            var result = await _postgresService.ExecuteNonQueryAsync(sql, parameters);
            return result > 0;
        }

        public async Task<bool> UpdateEmailAsync(string userId, string newEmail)
        {
            var sql = "UPDATE AspNetUsers SET Email = @Email, LastModifiedDate = @LastModifiedDate WHERE Id = @Id";
            var parameters = new[]
            {
                new NpgsqlParameter("@Email", newEmail),
                new NpgsqlParameter("@LastModifiedDate", DateTime.UtcNow),
                new NpgsqlParameter("@Id", userId)
            };

            var result = await _postgresService.ExecuteNonQueryAsync(sql, parameters);
            return result > 0;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var sql = "DELETE FROM AspNetUsers WHERE Id = @Id";
            var parameters = new[]
            {
                new NpgsqlParameter("@Id", userId)
            };

            var result = await _postgresService.ExecuteNonQueryAsync(sql, parameters);
            return result > 0;
        }

        public async Task<bool> SoftDeleteUserAsync(string userId)
        {
            var sql = "UPDATE AspNetUsers SET IsActive = false, LastModifiedDate = @LastModifiedDate WHERE Id = @Id";
            var parameters = new[]
            {
                new NpgsqlParameter("@LastModifiedDate", DateTime.UtcNow),
                new NpgsqlParameter("@Id", userId)
            };

            var result = await _postgresService.ExecuteNonQueryAsync(sql, parameters);
            return result > 0;
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string passwordHash)
        {
            var sql = "SELECT COUNT(1) FROM AspNetUsers WHERE UserName = @UserName AND PasswordHash = @PasswordHash AND IsActive = true";
            await using var connection = await _postgresService.GetConnectionAsync();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserName", username);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

            var result = (long)await command.ExecuteScalarAsync();
            return result > 0;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var sql = "SELECT COUNT(1) FROM AspNetUsers WHERE UserName = @UserName";
            await using var connection = await _postgresService.GetConnectionAsync();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserName", username);

            var result = (long)await command.ExecuteScalarAsync();
            return result > 0;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var sql = "SELECT COUNT(1) FROM AspNetUsers WHERE Email = @Email";
            await using var connection = await _postgresService.GetConnectionAsync();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);

            var result = (long)await command.ExecuteScalarAsync();
            return result > 0;
        }
    }
}
