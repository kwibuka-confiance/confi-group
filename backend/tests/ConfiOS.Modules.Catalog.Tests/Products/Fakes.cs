using ConfiOS.BuildingBlocks.Application.Auditing;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.Modules.Catalog.Application.Abstractions;
using ConfiOS.Modules.Catalog.Domain.Products;

namespace ConfiOS.Modules.Catalog.Tests.Products;

/// <summary>
/// In-memory product storage.
/// </summary>
/// <remarks>
/// Seeded products are keyed by their own id, exactly as the real repository finds
/// them. Keying by anything else would let a handler look a product up by one id
/// and act on another, which is precisely the confusion this guards against.
/// </remarks>
internal sealed class FakeProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _stored = [];

    public List<Product> Added { get; } = [];

    /// <summary>Forces <see cref="SkuExistsAsync"/> to report a clash.</summary>
    public bool SkuTaken { get; init; }

    /// <summary>The id last excluded from a uniqueness check, for asserting on.</summary>
    public Guid? LastSkuExclusion { get; private set; }

    public Product Seed(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        _stored[product.Id] = product;
        return product;
    }

    public Task<bool> SkuExistsAsync(
        string sku,
        Guid? excludingProductId = null,
        CancellationToken cancellationToken = default)
    {
        LastSkuExclusion = excludingProductId;
        return Task.FromResult(SkuTaken);
    }

    public Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_stored.GetValueOrDefault(id));

    public Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Product>>(Added);

    public void Add(Product product) => Added.Add(product);
}

internal sealed class FakeTenantContext : ITenantContext
{
    public bool IsResolved => true;

    public TenantId TenantId { get; } = TenantId.New();

    public BranchId? BranchId => null;

    public UserId? UserId => null;

    public string Locale => "en";

    public string CurrencyCode => "RWF";

    public string TimeZoneId => "Africa/Kigali";
}

internal sealed class FakeUnitOfWork : ICatalogUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }

    public Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IAsyncDisposable>(new NoopTransaction());

    private sealed class NoopTransaction : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

internal sealed class FakeAuditLogger : ICatalogAuditLogger
{
    public List<AuditEntry> Entries { get; } = [];

    public Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        Entries.Add(entry);
        return Task.CompletedTask;
    }
}
