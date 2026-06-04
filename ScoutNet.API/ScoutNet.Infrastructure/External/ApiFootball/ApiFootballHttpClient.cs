using ScoutNet.Infrastructure.Options;

namespace ScoutNet.Infrastructure.External.ApiFootball;

public static class ApiFootballHttpClient
{
    public const string Name = "ApiFootball";

    public static void ConfigureHttpClient(HttpClient client, ApiFootballOptions options)
    {
        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);

        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            client.DefaultRequestHeaders.Remove(options.ApiKeyHeaderName);
            client.DefaultRequestHeaders.Add(options.ApiKeyHeaderName, options.ApiKey);
        }

        if (options.SendApiHostHeader && !string.IsNullOrWhiteSpace(options.ApiHost))
        {
            client.DefaultRequestHeaders.Remove("x-rapidapi-host");
            client.DefaultRequestHeaders.Add("x-rapidapi-host", options.ApiHost);
        }
    }
}
