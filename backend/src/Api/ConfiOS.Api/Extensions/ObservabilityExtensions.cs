using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ConfiOS.Api.Extensions;

/// <summary>Configures OpenTelemetry tracing and metrics.</summary>
public static class ObservabilityExtensions
{
    /// <summary>Service name reported to the collector.</summary>
    public const string ServiceName = "confios-api";

    /// <summary>
    /// Adds tracing and metrics exported over OTLP. Without a configured endpoint the
    /// exporter is a no-op, so local development needs no collector running.
    /// </summary>
    public static IServiceCollection AddConfiOsObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(ServiceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                    // Health checks would otherwise dominate the trace volume.
                    options.Filter = context => !context.Request.Path.StartsWithSegments("/health"))
                .AddHttpClientInstrumentation()
                .AddOtlpExporter())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter());

        return services;
    }
}
