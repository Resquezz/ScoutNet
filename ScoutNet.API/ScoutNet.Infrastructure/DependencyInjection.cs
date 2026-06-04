using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ScoutNet.Application.Interfaces;
using ScoutNet.Application.Interfaces.External;
using ScoutNet.Application.Interfaces.Repositories;
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

        services.AddHttpClient<IFootballExternalService, FootballExternalService>((serviceProvider, client) =>
        {
            var apiFootballOptions = serviceProvider.GetRequiredService<IOptions<ApiFootballOptions>>().Value;
            ApiFootballHttpClient.ConfigureHttpClient(client, apiFootballOptions);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IWatchlistRepository, WatchlistRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
