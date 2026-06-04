using System.Text.Json.Serialization;

namespace ScoutNet.Infrastructure.External.ApiFootball.Models;

public class ApiFootballLeagueListItemDto
{
    [JsonPropertyName("league")]
    public ApiFootballLeagueInfoDto League { get; set; } = new();

    [JsonPropertyName("country")]
    public ApiFootballCountryDto? Country { get; set; }
}

public class ApiFootballLeagueInfoDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("logo")]
    public string? Logo { get; set; }
}

public class ApiFootballCountryDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("flag")]
    public string? Flag { get; set; }
}
