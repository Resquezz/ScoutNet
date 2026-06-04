using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using ScoutNet.Application.DTOs;

using ScoutNet.Application.Interfaces.Services;

using ScoutNet.Domain.Enums;

using ScoutNet.WebAPI.Authorization;



namespace ScoutNet.WebAPI.Controllers;



[ApiController]

[Route("api/players")]

public class PlayersController(IPlayerService playerService) : ControllerBase

{

    [HttpGet]

    [ProducesResponseType(typeof(IReadOnlyList<PlayerDto>), StatusCodes.Status200OK)]

    public async Task<ActionResult<IReadOnlyList<PlayerDto>>> GetPlayers(

        [FromQuery] int leagueId,

        [FromQuery] int season,

        [FromQuery] int? teamId,

        [FromQuery] string? searchTerm,

        [FromQuery] int? minAge,

        [FromQuery] int? maxAge,

        [FromQuery] PlayerPosition? position,

        [FromQuery] string? nationality,

        [FromQuery] int? minAppearances,

        [FromQuery] int? maxAppearances,

        [FromQuery] int? minGoals,

        [FromQuery] int? minAssists,

        [FromQuery] int? minShotsOn,

        [FromQuery] int? minPassAccuracy,

        [FromQuery] int? minDribblesSuccess,

        [FromQuery] int? minInterceptions,

        [FromQuery] int? minTackles,

        [FromQuery] bool forceSync = false,

        CancellationToken cancellationToken = default)

    {

        var filter = new PlayerFilterDto

        {

            SearchTerm = searchTerm,

            MinAge = minAge,

            MaxAge = maxAge,

            Position = position,

            Nationality = nationality,

            MinAppearances = minAppearances,

            MaxAppearances = maxAppearances,

            MinGoals = minGoals,

            MinAssists = minAssists,

            MinShotsOn = minShotsOn,

            MinPassAccuracy = minPassAccuracy,

            MinDribblesSuccess = minDribblesSuccess,

            MinInterceptions = minInterceptions,

            MinTackles = minTackles,

        };



        if (HasAdvancedFilters(filter) && !User.Identity?.IsAuthenticated == true)

        {

            return Unauthorized(new { title = "Unauthorized", detail = "Advanced search requires a Scout or Admin account." });

        }



        if (HasAdvancedFilters(filter))

        {

            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (role is not (AppRoles.Scout or AppRoles.Admin))

            {

                return Forbid();

            }

        }



        var players = await playerService.GetPlayersWithSyncAsync(

            filter,

            season,

            leagueId,

            teamId,

            forceSync,

            cancellationToken);



        return Ok(players);

    }



    [AllowAnonymous]

    [HttpGet("{id:int}")]

    [ProducesResponseType(typeof(PlayerDetailsDto), StatusCodes.Status200OK)]

    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public async Task<ActionResult<PlayerDetailsDto>> GetPlayerById(

        int id,

        CancellationToken cancellationToken)

    {

        var player = await playerService.GetPlayerDetailsAsync(id, cancellationToken);

        return player is null ? NotFound() : Ok(player);

    }



    [Authorize(Policy = AuthorizationPolicies.ScoutOrAdmin)]

    [HttpGet("compare")]

    [ProducesResponseType(typeof(PlayerComparisonDto), StatusCodes.Status200OK)]

    public async Task<ActionResult<PlayerComparisonDto>> ComparePlayers(

        [FromQuery] int id1,

        [FromQuery] int id2,

        [FromQuery] int season,

        CancellationToken cancellationToken)

    {

        var comparison = await playerService.ComparePlayersAsync(id1, id2, season, cancellationToken);

        return Ok(comparison);

    }



    private static bool HasAdvancedFilters(PlayerFilterDto filter) =>

        !string.IsNullOrWhiteSpace(filter.SearchTerm)

        || filter.MinAge.HasValue

        || filter.MaxAge.HasValue

        || filter.Position.HasValue

        || !string.IsNullOrWhiteSpace(filter.Nationality)

        || filter.MinAppearances.HasValue

        || filter.MaxAppearances.HasValue

        || filter.MinGoals.HasValue

        || filter.MinAssists.HasValue

        || filter.MinShotsOn.HasValue

        || filter.MinPassAccuracy.HasValue

        || filter.MinDribblesSuccess.HasValue

        || filter.MinInterceptions.HasValue

        || filter.MinTackles.HasValue;

}

