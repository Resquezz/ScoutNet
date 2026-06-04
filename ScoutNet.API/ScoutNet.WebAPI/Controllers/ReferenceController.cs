using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScoutNet.Infrastructure.Persistence;

namespace ScoutNet.WebAPI.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/reference")]
public class ReferenceController(ScoutDbContext dbContext) : ControllerBase
{
    [HttpGet("countries-leagues")]
    [ProducesResponseType(typeof(IReadOnlyList<CountryLeaguesDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CountryLeaguesDto>>> GetCountriesLeagues(CancellationToken cancellationToken)
    {
        var leagues = await dbContext.Leagues
            .AsNoTracking()
            .OrderBy(league => league.Country)
            .ThenBy(league => league.Name)
            .Select(league => new
            {
                league.Country,
                league.Flag,
                league.ExternalId,
                league.Name,
                league.Logo,
            })
            .ToListAsync(cancellationToken);

        var grouped = leagues
            .GroupBy(league => string.IsNullOrWhiteSpace(league.Country) ? "Other" : league.Country!)
            .Select(group => new CountryLeaguesDto
            {
                Country = group.Key,
                Flag = group.Select(item => item.Flag).FirstOrDefault(flag => !string.IsNullOrWhiteSpace(flag)),
                Leagues = group
                    .Select(item => new LeagueOptionDto
                    {
                        ExternalId = item.ExternalId,
                        Name = item.Name,
                        Country = string.IsNullOrWhiteSpace(item.Country) ? "Other" : item.Country!,
                        Flag = item.Flag,
                        Logo = item.Logo,
                    })
                    .ToList(),
            })
            .ToList();

        return Ok(grouped);
    }

    [HttpGet("teams")]
    [ProducesResponseType(typeof(IReadOnlyList<TeamOptionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TeamOptionDto>>> GetTeams(
        [FromQuery] int leagueId,
        CancellationToken cancellationToken)
    {
        var teams = await dbContext.Teams
            .AsNoTracking()
            .Where(team => team.ExternalLeagueId == leagueId)
            .OrderBy(team => team.Name)
            .Select(team => new TeamOptionDto
            {
                ExternalId = team.ExternalId,
                Name = team.Name,
                Logo = team.Logo,
            })
            .ToListAsync(cancellationToken);

        return Ok(teams);
    }

    public class CountryLeaguesDto
    {
        public string Country { get; set; } = string.Empty;

        public string? Flag { get; set; }

        public IReadOnlyList<LeagueOptionDto> Leagues { get; set; } = [];
    }

    public class LeagueOptionDto
    {
        public int ExternalId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string? Flag { get; set; }

        public string? Logo { get; set; }
    }

    public class TeamOptionDto
    {
        public int ExternalId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Logo { get; set; }
    }
}
