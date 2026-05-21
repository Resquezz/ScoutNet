using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScoutNet.Application.Interfaces;
using ScoutNet.Application.Interfaces.External;
using ScoutNet.Application.Interfaces.Repositories;
using ScoutNet.Application.Interfaces.Services;
using ScoutNet.Application.Services;
using ScoutNet.Application.Validators;
using ScoutNet.Infrastructure.External.ApiFootball;
using ScoutNet.Infrastructure.Options;
using ScoutNet.Infrastructure.Persistence;
using ScoutNet.Infrastructure.Repositories;

namespace ScoutNet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ApiFootballOptions>(configuration.GetSection(ApiFootballOptions.SectionName));

        services.AddDbContext<ScoutDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpClient(ApiFootballHttpClient.Name, (serviceProvider, client) =>
        {
            var options = serviceProvider
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiFootballOptions>>()
                .Value;
            ApiFootballHttpClient.ConfigureHttpClient(client, options);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IWatchlistRepository, WatchlistRepository>();
        services.AddScoped<IFootballExternalService, FootballExternalService>();

        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IReportService, ReportService>();

        services.AddValidatorsFromAssemblyContaining<CreateReportDtoValidator>();

        return services;
    }
}
