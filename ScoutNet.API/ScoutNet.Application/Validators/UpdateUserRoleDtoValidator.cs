using FluentValidation;
using ScoutNet.Application.DTOs.Auth;
using ScoutNet.Domain.Enums;

namespace ScoutNet.Application.Validators;

public class UpdateUserRoleDtoValidator : AbstractValidator<UpdateUserRoleDto>
{
    public UpdateUserRoleDtoValidator()
    {
        RuleFor(x => x.Role)
            .Must(role => role is UserRole.Scout or UserRole.Admin)
            .WithMessage("Role must be Scout or Admin.");
    }
}
