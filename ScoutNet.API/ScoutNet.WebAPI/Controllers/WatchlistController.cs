using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutNet.Application.DTOs;
using ScoutNet.Application.Interfaces.Services;
using ScoutNet.WebAPI.Authorization;

namespace ScoutNet.WebAPI.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.ScoutOrAdmin)]
[Route("api/watchlist")]
public class WatchlistController(IWatchlistService watchlistService) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WatchlistItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WatchlistItemDto>>> GetMyWatchlist(
        CancellationToken cancellationToken)
    {
        var items = await watchlistService.GetMyWatchlistAsync(GetCurrentUserId(), cancellationToken);
        return Ok(items);
    }

    [HttpPost("{playerId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPlayer(int playerId, CancellationToken cancellationToken)
    {
        await watchlistService.AddPlayerAsync(GetCurrentUserId(), playerId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{playerId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePlayer(int playerId, CancellationToken cancellationToken)
    {
        await watchlistService.RemovePlayerAsync(GetCurrentUserId(), playerId, cancellationToken);
        return NoContent();
    }
}
