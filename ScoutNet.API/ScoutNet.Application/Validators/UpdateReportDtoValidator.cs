using FluentValidation;
using ScoutNet.Application.DTOs;

namespace ScoutNet.Application.Validators;

public class UpdateReportDtoValidator : AbstractValidator<UpdateReportDto>
{
    public UpdateReportDtoValidator()
    {
        RuleFor(x => x.CurrentForm)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.Potential)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.Pros)
            .NotEmpty()
            .MinimumLength(10);

        RuleFor(x => x.Cons)
            .NotEmpty()
            .MinimumLength(10);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .MinimumLength(20);
    }
}
