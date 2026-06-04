using System.Text.Json.Serialization;

namespace ScoutNet.Infrastructure.External.ApiFootball.Models;

public class ApiFootballTeamListItemDto
{
    [JsonPropertyName("team")]
    public ApiFootballTeamDto Team { get; set; } = new();
}
