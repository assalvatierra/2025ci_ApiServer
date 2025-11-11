using Microsoft.AspNetCore.Authorization;

namespace ApiServer.MinimalAPI
{
    public static class Sample
    {
        public static WebApplication MapSampleEndpoints(this WebApplication app)
        {
            var sampleApiGroup = app.MapGroup("api/sample");
            sampleApiGroup.MapGet("", getSampleMessageAsync);
            sampleApiGroup.MapGet("{Id}", getSampleDataAsync);

            // New endpoint that accepts a string parameter
            sampleApiGroup.MapGet("byname/{name}", getSampleByNameAsync);

            return app;
        }

        private static async Task<IResult> getSampleMessageAsync()
        {
            await Task.Delay(10); // Simulate async work
            return Results.Ok(new { message = "Sample Minimal API async is working!" });
        }

        private static async Task<IResult> getSampleDataAsync(int Id)
        {
            await Task.Delay(10); // Simulate async work
            var sampleData = new
            {
                Id = 1,
                Name = "Sample Data",
                Description = "This is a sample data object from Minimal API with param:" + Id.ToString()
            };
            return Results.Ok(sampleData);
        }

        // New handler for string parameter
        private static async Task<IResult> getSampleByNameAsync(string name)
        {
            await Task.Delay(10); // Simulate async work
            var sampleData = new
            {
                Name = name,
                Message = $"Hello, {name}! This is a sample response with a string parameter."
            };
            return Results.Ok(sampleData);
        }


    }
}
