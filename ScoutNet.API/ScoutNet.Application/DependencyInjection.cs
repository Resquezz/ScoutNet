using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ScoutNet.Application.Interfaces.Services;
using ScoutNet.Application.Services;
using ScoutNet.Application.Validators;

namespace ScoutNet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddValidatorsFromAssemblyContaining<CreateReportDtoValidator>();

        return services;
    }
}
