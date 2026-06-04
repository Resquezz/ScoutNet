using ScoutNet.Application.DTOs.Auth;
using ScoutNet.Domain.Enums;

namespace ScoutNet.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    Task<UserProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserProfileDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    Task<UserProfileDto> UpdateUserRoleAsync(
        Guid userId,
        UserRole role,
        CancellationToken cancellationToken = default);
}
