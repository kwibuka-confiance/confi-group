using System.Reflection;
using ConfiOS.Api.Extensions;
using ConfiOS.BuildingBlocks.Api;
using ConfiOS.BuildingBlocks.Api.Modules;
using ConfiOS.BuildingBlocks.Application;
using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Infrastructure.Interceptors;
using ConfiOS.BuildingBlocks.Infrastructure.Time;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Structured logging from the first line, so startup failures are diagnosable too.
builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddBuildingBlocksApplication();
builder.Services.AddBuildingBlocksApi();

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<AuditingInterceptor>();

builder.Services.AddConfiOsAuthentication(builder.Configuration);
builder.Services.AddConfiOsObservability(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

// Each module registers its own services. The host names the assemblies but knows nothing
// about what is inside them.
builder.Services.AddModules(
    builder.Configuration,
    typeof(ConfiOS.Modules.Identity.Api.IdentityModule).Assembly,
    typeof(ConfiOS.Modules.Catalog.Api.CatalogModule).Assembly,
    typeof(ConfiOS.Modules.Inventory.Api.InventoryModule).Assembly,
    typeof(ConfiOS.Modules.Sales.Api.SalesModule).Assembly);

var app = builder.Build();

// Order matters. Exceptions are caught outermost so nothing escapes unformatted; language
// is negotiated before any message is produced; tenant context is resolved only after
// authentication has established who the caller is.
app.UseBuildingBlocksApi();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();
app.UseTenantResolution();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.MapModules();

await app.RunAsync().ConfigureAwait(false);

/// <summary>
/// Exposed so integration tests can start the host with WebApplicationFactory. The rest of
/// the class is generated from the top-level statements above.
/// </summary>
public partial class Program
{
    protected Program()
    {
    }
}
