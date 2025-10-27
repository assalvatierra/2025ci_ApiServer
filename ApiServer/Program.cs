using ApiServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "Redis";
});

builder.Services.AddScoped<ApiServer.RedisCacheService>();

builder.Services.AddControllers();
// Add this line if not already present:
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

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

// Inject Custom Services
builder.Services.AddScoped<SystemUserServices>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
