using System.Net;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shared;

namespace Frontend.Services;

public class RenewBackendUrlHandler(IWebAssemblyHostEnvironment webAssemblyHostEnvironment) : HttpClientHandler {
    public string? BackendUrl;
    private readonly IWebAssemblyHostEnvironment _webAssemblyHostEnvironment = webAssemblyHostEnvironment;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        var host = (request.RequestUri ?? throw new NullReferenceException()).Host;

        if (host is not "lhr.life") {
            return await base.SendAsync(request, cancellationToken);
        }

        BackendUrl ??= await GetBackendUrl(cancellationToken);
        request.RequestUri = new Uri($"{BackendUrl}{request.RequestUri.PathAndQuery}");
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode is not HttpStatusCode.ServiceUnavailable) {
            return response;
        }

        BackendUrl = await GetBackendUrl(cancellationToken);
        request.RequestUri = new Uri($"{BackendUrl}{request.RequestUri.PathAndQuery}");
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string?> GetBackendUrl(CancellationToken cancellationToken) {
        using var httpClient = new HttpClient();
        return await BackendUrlResolver.GetUrl(httpClient, _webAssemblyHostEnvironment.IsDevelopment(), cancellationToken);
    }
}