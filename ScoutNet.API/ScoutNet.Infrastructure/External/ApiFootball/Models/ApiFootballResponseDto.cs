using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutNet.Infrastructure.External.ApiFootball.Models;

public class ApiFootballResponseDto<T>
{
    [JsonPropertyName("get")]
    public string? Get { get; set; }

    /// <summary>Query parameters echoed by the API (values may be string or number).</summary>
    [JsonPropertyName("parameters")]
    public JsonElement Parameters { get; set; }

    [JsonPropertyName("errors")]
    public JsonElement Errors { get; set; }

    [JsonPropertyName("results")]
    public int Results { get; set; }

    [JsonPropertyName("paging")]
    public ApiFootballPagingDto Paging { get; set; } = new();

    [JsonPropertyName("response")]
    public List<T> Response { get; set; } = [];
}
