using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Infrastructure.Persistence;
using ConfiOS.BuildingBlocks.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConfiOS.Modules.Catalog.Infrastructure.Persistence;

/// <summary>
/// Lets the EF Core tools build <see cref="CatalogDbContext"/> without booting the API host.
/// The connection string comes from the <c>ConnectionStrings__Postgres</c> environment
/// variable, falling back to the local development database.
/// </summary>
public sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=confios;Username=confios;Password=confios";

    public CatalogDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Postgres") ?? DefaultConnectionString;

        var builder = new DbContextOptionsBuilder<CatalogDbContext>();
        builder.UseConfiOsPostgres(connectionString, CatalogDbContext.SchemaName);

        return new CatalogDbContext(builder.Options, new AmbientContext(), new SystemClock());
    }
}
