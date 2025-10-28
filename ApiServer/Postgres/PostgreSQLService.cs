using Microsoft.Extensions.Options;
using Npgsql;

namespace ApiServer.Postgres
{
    public class PostgreSQLService
    {
        private readonly PostgreSQLSettings _settings;
        private readonly ILogger<PostgreSQLService> _logger;

        public PostgreSQLService(IOptions<PostgreSQLSettings> settings, ILogger<PostgreSQLService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<NpgsqlConnection> GetConnectionAsync()
        {
            var connection = new NpgsqlConnection(_settings.ConnectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<T> ExecuteQueryAsync<T>(string query, Func<NpgsqlDataReader, T> mapper, NpgsqlParameter[]? parameters = null)
        {
            using var connection = await GetConnectionAsync();
            using var command = new NpgsqlCommand(query, connection);
            
            if (parameters != null)
                command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return mapper(reader);
            }

            return default(T);
        }

        public async Task<int> ExecuteNonQueryAsync(string query, NpgsqlParameter[]? parameters = null)
        {
            using var connection = await GetConnectionAsync();
            using var command = new NpgsqlCommand(query, connection);
            
            if (parameters != null)
                command.Parameters.AddRange(parameters);

            return await command.ExecuteNonQueryAsync();
        }
    }
}
