using System.Globalization;
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
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture));

builder.Services.AddBuildingBlocksApplication();
builder.Services.AddBuildingBlocksApi();

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<AuditingInterceptor>();

builder.Services.AddConfiOsAuthentication(builder.Configuration);
builder.Services.AddConfiOsObservability(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

// Development-only CORS so the Flutter web client (served from another origin) can call the
// API from a browser. Production origins are configured explicitly, never wide open.
const string DevelopmentCorsPolicy = "development";
builder.Services.AddCors(options => options.AddPolicy(
    DevelopmentCorsPolicy,
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Each module registers its own services. The host names the assemblies but knows nothing
// about what is inside them.
builder.Services.AddModules(
    builder.Configuration,
    typeof(ConfiOS.Modules.Identity.Api.IdentityModule).Assembly,
    typeof(ConfiOS.Modules.Catalog.Api.CatalogModule).Assembly,
    typeof(ConfiOS.Modules.Inventory.Api.InventoryModule).Assembly,
    typeof(ConfiOS.Modules.Sales.Api.SalesModule).Assembly);

// Events are written to each module's outbox inside the transaction that raised them; this
// is what actually delivers them. Without it every event is recorded and never heard.
builder.Services.AddScoped<ConfiOS.BuildingBlocks.Infrastructure.Outbox.OutboxDispatcher>();
builder.Services.Configure<ConfiOS.BuildingBlocks.Infrastructure.Outbox.OutboxOptions>(
    builder.Configuration.GetSection("Outbox"));
builder.Services.AddHostedService<ConfiOS.BuildingBlocks.Infrastructure.Outbox.OutboxPublisher>();

var app = builder.Build();

// Order matters. Exceptions are caught outermost so nothing escapes unformatted; language
// is negotiated before any message is produced; tenant context is resolved only after
// authentication has established who the caller is.
app.UseBuildingBlocksApi();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseCors(DevelopmentCorsPolicy);
}

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
