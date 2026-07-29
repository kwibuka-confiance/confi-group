using Microsoft.EntityFrameworkCore;

namespace ConfiOS.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// One place that decides how every module talks to PostgreSQL, so the provider, the
/// per-schema migrations history table and the naming convention stay identical across
/// modules instead of being repeated (and able to drift) in each registration.
/// </summary>
public static class ConfiOsDbContextOptionsExtensions
{
    /// <summary>
    /// Configures the Npgsql provider for a module, keeping its migrations history in the
    /// module's own schema and mapping tables and columns to snake_case, which is the
    /// idiomatic casing for PostgreSQL and avoids quoted-identifier surprises.
    /// </summary>
    /// <param name="options">The options builder being configured.</param>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
    /// <param name="schema">The module's schema, which also names its migrations table.</param>
    public static DbContextOptionsBuilder UseConfiOsPostgres(
        this DbContextOptionsBuilder options,
        string? connectionString,
        string schema)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.UseNpgsql(
            connectionString,
            npgsql => npgsql.MigrationsHistoryTable("__migrations", schema));
        options.UseSnakeCaseNamingConvention();

        return options;
    }
}
