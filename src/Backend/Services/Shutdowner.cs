using Shared;
using Shared.Models;

namespace Backend.Services;

public class Shutdowner : BackgroundService {
    private readonly ILogger<Shutdowner> _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public Shutdowner(ILogger<Shutdowner> logger, IHostEnvironment hostEnvironment, IServiceProvider serviceProvider,
        IHostApplicationLifetime applicationLifetime) {
        _logger = logger;
        _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        _serviceProvider = serviceProvider;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        if (_hostEnvironment.IsDevelopment()) {
            _logger.LogInformation("Development environment, so Shutdowner worker is not needed");
            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

        while (!stoppingToken.IsCancellationRequested) {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            try {
                var backendUrl = await BackendUrlResolver.GetUrl(httpClient, _hostEnvironment.IsDevelopment(), stoppingToken);
                var appInfo = await httpClient.GetFromJsonAsync<ApplicationInfoDto>(backendUrl, cancellationToken: stoppingToken);
                if (appInfo != null && Constants.ApplicationStartDate >= appInfo.ApplicationStartDate) {
                    continue;
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Something went wrong");
                continue;
            }

            _logger.LogInformation("There is a new instance of application was started, so shutdown this one");
            _applicationLifetime.StopApplication();
        }
    }
}