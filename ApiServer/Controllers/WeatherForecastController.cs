using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using System.Text.Json;

namespace ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly RedisCacheService _cache;
    private readonly ApiServer.Mongo.MongoDBSettings _mongoSettings;
    public bool IsFromCache { get; set; } = false;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, RedisCacheService cache, IOptions<ApiServer.Mongo.MongoDBSettings> mongoOptions)
    {
        _logger = logger;
        _cache = cache;
        _mongoSettings = mongoOptions?.Value ?? throw new ArgumentNullException(nameof(mongoOptions));
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        string cacheKey = $"weatherForecastData_{System.DateTime.Now:yyyyMMdd_HHmm}";
        var cachedData = _cache.GetCachedData<WeatherForecast[]>(cacheKey);
        if (cachedData != null)
        {
            IsFromCache = true;
            return cachedData;
        }
        else
        {

            var Data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
            _cache.SetCachedData(cacheKey, Data, TimeSpan.FromMinutes(1));
            return Data;

        }

    }

    [HttpPost]
    [Route("save")]
    public IEnumerable<WeatherForecast> SaveWeatherForecast([FromBody] WeatherForecastDto forecastDto)
    {
        // Here you would typically save the forecast to a database or other storage.
        var Data = new WeatherForecast[] {
            new WeatherForecast
            {
                Date = forecastDto.Date,
                TemperatureC = forecastDto.TemperatureC,
                Summary = forecastDto.Summary,
                GenerationType = "Test"
            }
        };


        // Save to MongoDB
        try
        {
            foreach (var item in Data)
            {
                AddToMongoDB(item);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save weather forecast to MongoDB");
            return Data;
        }

        _logger.LogInformation("Received Weather Forecast: {@Forecast}", Data);
        return Data;
    }

    private void AddToMongoDB(WeatherForecast forecast)
    {
        if (_mongoSettings == null || string.IsNullOrWhiteSpace(_mongoSettings.ConnectionURI))
            throw new ArgumentNullException("MongoDB:ConnectionURI is not configured");

        var client = new MongoClient(_mongoSettings.ConnectionURI);
        var database = client.GetDatabase(_mongoSettings.DatabaseName ?? "test");
        var collection = database.GetCollection<BsonDocument>("weatherforecast");

        // Serialize forecast to JSON then parse into a BsonDocument for insertion
        var json = JsonSerializer.Serialize(forecast, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var doc = BsonDocument.Parse(json);
        collection.InsertOne(doc);
    }

}

public class WeatherForecastDto
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; } = null!;
}
