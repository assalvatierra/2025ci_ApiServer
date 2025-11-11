using Microsoft.Extensions.DependencyInjection;

namespace ApiServer.Services
{
    public static class AppDependencies
    {
        public static WebApplicationBuilder addDependencies(this WebApplicationBuilder builder)
        {
            // Add Redis services
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "Redis";
            });
            builder.Services.AddScoped<ApiServer.RedisCacheService>();


            // Register MongoDB settings and service. The settings are nested under ConnectionStrings:MongoDB in appsettings.json.
            builder.Services.Configure<ApiServer.Mongo.MongoDBSettings>(builder.Configuration.GetSection("ConnectionStrings:MongoDB"));
            builder.Services.AddSingleton<ApiServer.Mongo.MongodbService>();

            // Validate that MongoDB connection URI exists at startup to avoid obscure NullReference exceptions later.
            var mongoSettings = builder.Configuration.GetSection("ConnectionStrings:MongoDB").Get<ApiServer.Mongo.MongoDBSettings>();
            if (mongoSettings == null || string.IsNullOrWhiteSpace(mongoSettings.ConnectionURI))
            {
                // Log and throw early so the developer sees a clear message
                var logger = LoggerFactory.Create(lb => lb.AddConsole()).CreateLogger("Startup");
                logger.LogError("MongoDB connection settings are missing or invalid. Check appsettings.json ConnectionStrings:MongoDB section.");
                throw new ArgumentNullException("MongoDB:ConnectionURI", "MongoDB connection string is not configured. Check appsettings.json ConnectionStrings:MongoDB.ConnectionURI");
            }


            //PostGres Service Registration
            builder.Services.AddScoped<ApiServer.Postgres.PostgreSQLService>();
            builder.Services.Configure<ApiServer.Postgres.PostgreSQLSettings>(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                // Set other properties if needed, e.g.:
                // options.CommandTimeout = 30;
                // options.MaxPoolSize = 100;
            });
            builder.Services.AddScoped<ApiServer.Postgres.Repository.IAspNetUserRepo, ApiServer.Postgres.Repository.AspNetUserRepo>();


            builder.Services.AddScoped<SystemUserServices>();
            return builder;
        }
    }
}
