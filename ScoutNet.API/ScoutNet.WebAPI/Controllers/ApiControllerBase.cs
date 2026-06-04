using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ScoutNet.Domain.Enums;

namespace ScoutNet.WebAPI.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.Parse(userId!);
    }

    protected UserRole GetCurrentUserRole()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        return Enum.TryParse<UserRole>(role, out var parsed) ? parsed : UserRole.Guest;
    }

    protected bool IsAdmin => GetCurrentUserRole() == UserRole.Admin;
}
