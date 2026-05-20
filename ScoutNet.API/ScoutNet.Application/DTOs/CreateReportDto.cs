namespace ScoutNet.Application.DTOs;

public class CreateReportDto
{
    public Guid PlayerId { get; set; }

    public int CurrentForm { get; set; }

    public int Potential { get; set; }

    public string Pros { get; set; } = string.Empty;

    public string Cons { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;
}
