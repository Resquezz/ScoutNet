using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ScoutNet.Infrastructure.Persistence;

public class ScoutDbContextFactory : IDesignTimeDbContextFactory<ScoutDbContext>
{
    public ScoutDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string not found. Add ConnectionStrings:DefaultConnection to ScoutNet.WebAPI/appsettings.json " +
                "or set environment variable ConnectionStrings__DefaultConnection.");

        var optionsBuilder = new DbContextOptionsBuilder<ScoutDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ScoutDbContext(optionsBuilder.Options);
    }

    private static IConfiguration BuildConfiguration()
    {
        var builder = new ConfigurationBuilder().AddEnvironmentVariables();

        var configRoots = DiscoverConfigRoots().ToList();
        var webApiRoot = configRoots.FirstOrDefault(path =>
            path.EndsWith("ScoutNet.WebAPI", StringComparison.OrdinalIgnoreCase) &&
            File.Exists(Path.Combine(path, "appsettings.json")));

        if (webApiRoot is not null && TryAddAppSettings(builder, webApiRoot))
        {
            return builder.Build();
        }

        foreach (var root in configRoots)
        {
            if (TryAddAppSettings(builder, root))
            {
                return builder.Build();
            }
        }

        return builder.Build();
    }

    private static IEnumerable<string> DiscoverConfigRoots()
    {
        var roots = new List<string>();
        var directory = Directory.GetCurrentDirectory();

        for (var depth = 0; depth < 6 && directory is not null; depth++)
        {
            roots.Add(Path.Combine(directory, "ScoutNet.WebAPI"));
            roots.Add(directory);
            directory = Directory.GetParent(directory)?.FullName;
        }

        return roots.Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static bool TryAddAppSettings(IConfigurationBuilder builder, string root)
    {
        if (!Directory.Exists(root))
        {
            return false;
        }

        var baseSettings = Path.Combine(root, "appsettings.json");
        if (!File.Exists(baseSettings))
        {
            return false;
        }

        builder.SetBasePath(root)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

        var developmentSettings = Path.Combine(root, "appsettings.Development.json");
        if (File.Exists(developmentSettings))
        {
            builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
        }

        return true;
    }
}
