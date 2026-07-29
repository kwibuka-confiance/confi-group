using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConfiOS.Modules.Identity.Infrastructure.Persistence;

/// <summary>
/// Lets the EF Core command-line tools build <see cref="IdentityDbContext"/> without booting
/// the API host. The module's context lives in a class library, so <c>dotnet ef</c> needs a
/// design-time factory to add or apply migrations. Runtime wiring is unaffected: the host
/// still supplies the real options, tenant context and clock.
/// </summary>
/// <remarks>
/// The connection string is read from the <c>ConnectionStrings__Postgres</c> environment
/// variable, falling back to the local development database. Adding a migration does not
/// connect; applying one does.
/// </remarks>
public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=confios;Username=confios;Password=confios";

    public IdentityDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Postgres") ?? DefaultConnectionString;

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__migrations", IdentityDbContext.SchemaName))
            .Options;

        // Design time never resolves a tenant: an unresolved AmbientContext is enough to build
        // the model, and the tenant query filters read the tenant lazily at query time.
        return new IdentityDbContext(options, new AmbientContext(), new SystemClock());
    }
}
