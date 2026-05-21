namespace ScoutNet.Application.DTOs;

public class TeamDto
{
    public int ExternalId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Logo { get; set; }
}
