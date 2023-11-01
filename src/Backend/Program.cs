using Shared.Models;

var app = WebApplication.CreateBuilder()
    .AddShutdowner()
    .AddCors()
    .AddHttpClient()
    .Build();

app.UseCors();
app.UseRouting();
app.UseMetricServer();
app.MapGet("/", () => new ApplicationInfoDto(Constants.ApplicationStartDate));

app.Run();