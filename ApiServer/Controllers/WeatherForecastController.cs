using Microsoft.AspNetCore.Mvc;

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
    public bool IsFromCache { get; set; } = false;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, RedisCacheService cache)
    {
        _logger = logger;
        _cache = cache;
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

    [HttpPost(Name = "SaveWeatherForecast")]
    public IActionResult SaveWeatherForecast([FromBody] WeatherForecast forecast)
    {
        // Here you would typically save the forecast to a database or other storage.
        // For this example, we'll just log it and return a success response.
        _logger.LogInformation("Received Weather Forecast: {@Forecast}", forecast);
        return Ok(new { Message = "Weather forecast saved successfully." });
    }



}
