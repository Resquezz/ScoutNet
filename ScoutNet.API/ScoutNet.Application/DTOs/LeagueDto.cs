namespace ScoutNet.Application.DTOs;

public class LeagueDto
{
    public int ExternalId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Country { get; set; }

    public string? Logo { get; set; }

    public string? Flag { get; set; }
}
