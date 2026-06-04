using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutNet.Application.DTOs;
using ScoutNet.Application.Interfaces.Services;
using ScoutNet.WebAPI.Authorization;

namespace ScoutNet.WebAPI.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.ScoutOrAdmin)]
[Route("api/reports")]
public class ReportsController(IReportService reportService) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ScoutReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ScoutReportDto>>> GetReports(
        [FromQuery] int? playerId,
        CancellationToken cancellationToken)
    {
        var reports = await reportService.GetReportsAsync(
            GetCurrentUserId(),
            GetCurrentUserRole(),
            playerId,
            cancellationToken);

        return Ok(reports);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ScoutReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScoutReportDto>> GetReport(Guid id, CancellationToken cancellationToken)
    {
        var report = await reportService.GetByIdAsync(
            id,
            GetCurrentUserId(),
            GetCurrentUserRole(),
            cancellationToken);

        return report is null ? NotFound() : Ok(report);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ScoutReportDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ScoutReportDto>> CreateReport(
        [FromBody] CreateReportDto request,
        CancellationToken cancellationToken)
    {
        var report = await reportService.CreateAsync(request, GetCurrentUserId(), cancellationToken);
        return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ScoutReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScoutReportDto>> UpdateReport(
        Guid id,
        [FromBody] UpdateReportDto request,
        CancellationToken cancellationToken)
    {
        var report = await reportService.UpdateAsync(id, request, GetCurrentUserId(), cancellationToken);
        return Ok(report);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReport(Guid id, CancellationToken cancellationToken)
    {
        await reportService.DeleteAsync(id, GetCurrentUserId(), GetCurrentUserRole(), cancellationToken);
        return NoContent();
    }
}
