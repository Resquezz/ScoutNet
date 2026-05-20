using FluentValidation;
using ScoutNet.Application.DTOs;

namespace ScoutNet.Application.Validators;

public class CreateReportDtoValidator : AbstractValidator<CreateReportDto>
{
    public CreateReportDtoValidator()
    {
        RuleFor(x => x.CurrentForm)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.Potential)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.Pros)
            .NotEmpty();

        RuleFor(x => x.Cons)
            .NotEmpty();

        RuleFor(x => x.Summary)
            .NotEmpty()
            .MinimumLength(20);
    }
}
