using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoutNet.Application.DTOs.Auth;
using ScoutNet.Application.Interfaces.Services;
using ScoutNet.WebAPI.Authorization;

namespace ScoutNet.WebAPI.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/admin/users")]
public class AdminUsersController(IAuthService authService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserProfileDto>>> GetUsers(
        CancellationToken cancellationToken)
    {
        var users = await authService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPatch("{id:guid}/role")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDto>> UpdateUserRole(
        Guid id,
        [FromBody] UpdateUserRoleDto request,
        CancellationToken cancellationToken)
    {
        var user = await authService.UpdateUserRoleAsync(id, request.Role, cancellationToken);
        return Ok(user);
    }
}
