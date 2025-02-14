using Shared.Models;

var app = WebApplication.CreateBuilder()
    .AddShutdowner()
    .AddCors()
    .AddHttpClient()
    .Build();

app.UseCors(builder => builder
    .WithOrigins("https://capybara-pro-code.github.io")
    .AllowAnyHeader()
    .AllowAnyMethod());
app.UseRouting();
app.UseMetricServer();
app.MapGet("/", () => new ApplicationInfoDto(Constants.ApplicationStartDate));

app.Run();