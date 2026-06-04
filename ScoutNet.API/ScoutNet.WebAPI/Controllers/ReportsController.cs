using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutNet.Application.DTOs;
using ScoutNet.Application.Interfaces.Services;

namespace ScoutNet.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/reports")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ScoutReportDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ScoutReportDto>> CreateReport(
        [FromBody] CreateReportDto request,
        CancellationToken cancellationToken)
    {
        var scoutId = GetCurrentUserId();
        var report = await reportService.CreateAsync(request, scoutId, cancellationToken);
        return CreatedAtAction(nameof(CreateReport), new { id = report.Id }, report);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReport(Guid id, CancellationToken cancellationToken)
    {
        var scoutId = GetCurrentUserId();
        await reportService.DeleteAsync(id, scoutId, cancellationToken);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.Parse(userId!);
    }
}
