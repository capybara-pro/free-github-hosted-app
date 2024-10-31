namespace Shared;

public static class BackendUrlResolver {
    public static async Task<string> GetUrl(HttpClient client, bool isDevelopment, CancellationToken cancellationToken = default) {
        if (isDevelopment) {
            return "http://localhost:5000";
        }

        return await client.GetStringAsync($"{Constants.TunnelUrl}?token={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}", cancellationToken);
    }
}