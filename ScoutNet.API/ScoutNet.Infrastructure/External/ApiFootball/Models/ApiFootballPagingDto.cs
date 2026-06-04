using System.Text.Json.Serialization;

namespace ScoutNet.Infrastructure.External.ApiFootball.Models;

public class ApiFootballPagingDto
{
    [JsonPropertyName("current")]
    public int Current { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}
