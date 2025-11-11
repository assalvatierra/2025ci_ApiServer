using ApiServer.Services;
using ApiServer.MinimalAPI;
using System.Runtime.CompilerServices;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add this line if not already present:
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);


builder.addDependencies();


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

// No changes required unless you want to register a custom route constraint.
// The 'string' constraint is not built-in and is unnecessary for route parameters of type string.
// If you need a custom constraint, register it here using:
// builder.Services.Configure<RouteOptions>(options => { options.ConstraintMap.Add("yourConstraint", typeof(YourConstraintType)); });

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

//var sampleApiGroup = app.MapGroup("api/sample");
//sampleApiGroup.MapGet("/", () => "API Server is running....");
app.MapSampleEndpoints();

app.Run();
