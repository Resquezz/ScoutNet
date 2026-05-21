using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutNet.Infrastructure.External.ApiFootball.Models;

public class ApiFootballResponseDto<T>
{
    [JsonPropertyName("get")]
    public string? Get { get; set; }

    [JsonPropertyName("parameters")]
    public Dictionary<string, string>? Parameters { get; set; }

    [JsonPropertyName("errors")]
    public JsonElement Errors { get; set; }

    [JsonPropertyName("results")]
    public int Results { get; set; }

    [JsonPropertyName("paging")]
    public ApiFootballPagingDto Paging { get; set; } = new();

    [JsonPropertyName("response")]
    public List<T> Response { get; set; } = [];
}
