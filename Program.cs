using Demo;
using Microsoft.Extensions.Logging.Console;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders().AddConsole().AddCustomFormatter(o =>
    {
        o.CustomPrefix = Environment.NewLine + " >>> ";
        o.ColorBehavior = LoggerColorBehavior.Default;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", async context =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    Log.HelloWorld(logger);
    await context.Response.WriteJsonAsync(nameof(Log.HelloWorld));
})
.WithName("Index");

app.MapGet("/weatherforecast", async context =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    foreach (WeatherForecast forecastOnDay in forecast)
    {
        Log.GotWeatherForecastWithSummary(logger, forecastOnDay.Summary!);
    }
    string jsonString = JsonSerializer.Serialize(forecast);
    await context.Response.WriteJsonAsync(jsonString);
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}