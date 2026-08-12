using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Events;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace ConfiOS.BuildingBlocks.Tests.Outbox;

public sealed class OutboxDispatcherTests
{
    private sealed record OrderPlaced(Guid OrderId) : DomainEvent("test.order-placed");

    private sealed record NobodyListens() : DomainEvent("test.nobody-listens");

    /// <summary>Records what it saw, including the tenant it was handled as.</summary>
    private sealed class RecordingHandler(ITenantContext tenantContext) : IDomainEventHandler<OrderPlaced>
    {
        public static readonly List<(Guid OrderId, Guid? TenantId)> Seen = [];

        public Task HandleAsync(OrderPlaced domainEvent, CancellationToken cancellationToken)
        {
            Seen.Add((domainEvent.OrderId, tenantContext.IsResolved ? tenantContext.TenantId.Value : null));
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingHandler : IDomainEventHandler<OrderPlaced>
    {
        public Task HandleAsync(OrderPlaced domainEvent, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("handler blew up");
    }

    private static ServiceProvider Build(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        services.AddScoped<AmbientContext>();
        services.AddScoped<ITenantContext>(provider => provider.GetRequiredService<AmbientContext>());
        configure(services);
        return services.BuildServiceProvider();
    }

    private static OutboxDispatcher DispatcherFor(ServiceProvider provider) =>
        new(provider.GetRequiredService<IServiceScopeFactory>());

    [Fact]
    public async Task A_handler_runs_as_the_tenant_the_event_belongs_to()
    {
        RecordingHandler.Seen.Clear();
        var tenant = Guid.CreateVersion7();
        var orderId = Guid.CreateVersion7();

        await using var provider = Build(services =>
            services.AddScoped<IDomainEventHandler<OrderPlaced>, RecordingHandler>());

        var invoked = await DispatcherFor(provider)
            .DispatchAsync(new OrderPlaced(orderId), tenant, CancellationToken.None);

        invoked.ShouldBe(1);
        RecordingHandler.Seen.ShouldHaveSingleItem().ShouldBe((orderId, tenant));
    }

    [Fact]
    public async Task A_platform_event_is_handled_with_no_tenant_resolved()
    {
        RecordingHandler.Seen.Clear();

        await using var provider = Build(services =>
            services.AddScoped<IDomainEventHandler<OrderPlaced>, RecordingHandler>());

        await DispatcherFor(provider)
            .DispatchAsync(new OrderPlaced(Guid.CreateVersion7()), tenantId: null, CancellationToken.None);

        RecordingHandler.Seen.ShouldHaveSingleItem().TenantId.ShouldBeNull();
    }

    [Fact]
    public async Task Every_handler_registered_for_the_event_runs()
    {
        RecordingHandler.Seen.Clear();

        await using var provider = Build(services =>
        {
            services.AddScoped<IDomainEventHandler<OrderPlaced>, RecordingHandler>();
            services.AddScoped<IDomainEventHandler<OrderPlaced>, RecordingHandler>();
        });

        var invoked = await DispatcherFor(provider)
            .DispatchAsync(new OrderPlaced(Guid.CreateVersion7()), Guid.CreateVersion7(), CancellationToken.None);

        invoked.ShouldBe(2);
        RecordingHandler.Seen.Count.ShouldBe(2);
    }

    [Fact]
    public async Task An_event_nobody_handles_is_not_an_error()
    {
        await using var provider = Build(_ => { });

        var invoked = await DispatcherFor(provider)
            .DispatchAsync(new NobodyListens(), Guid.CreateVersion7(), CancellationToken.None);

        invoked.ShouldBe(0);
    }

    [Fact]
    public async Task A_failing_handler_surfaces_so_the_message_can_be_retried()
    {
        await using var provider = Build(services =>
            services.AddScoped<IDomainEventHandler<OrderPlaced>, ThrowingHandler>());

        await Should.ThrowAsync<InvalidOperationException>(() =>
            DispatcherFor(provider).DispatchAsync(
                new OrderPlaced(Guid.CreateVersion7()),
                Guid.CreateVersion7(),
                CancellationToken.None));
    }
}
