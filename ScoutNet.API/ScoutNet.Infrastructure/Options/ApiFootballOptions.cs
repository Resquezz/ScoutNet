namespace ScoutNet.Infrastructure.Options;

public class ApiFootballOptions
{
    public const string SectionName = "ApiFootball";

    public string BaseUrl { get; set; } = "https://v3.football.api-sports.io/";

    public string ApiKey { get; set; } = string.Empty;

    public string ApiHost { get; set; } = "v3.football.api-sports.io";

    /// <summary>
    /// Use "x-apisports-key" for keys from https://dashboard.api-sports.io/
    /// or "x-rapidapi-key" when using RapidAPI.
    /// </summary>
    public string ApiKeyHeaderName { get; set; } = "x-apisports-key";

    public bool SendApiHostHeader { get; set; } = true;

    public string LeaguesEndpoint { get; set; } = "leagues";

    public string TeamsEndpoint { get; set; } = "teams";

    public string PlayersEndpoint { get; set; } = "players";

    /// <summary>Max page number per request (API-Football free plan allows up to 3).</summary>
    public int MaxPages { get; set; } = 3;

    public int RequestTimeoutSeconds { get; set; } = 100;
}
