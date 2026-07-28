using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// A postal or physical address. The administrative levels follow Rwanda's structure
/// (province, district, sector, cell, village) because that is what the first customers
/// use; all of them are optional so other countries can leave them empty and rely on
/// <see cref="Country"/> and <see cref="Street"/>.
/// </summary>
public sealed class Address : ValueObject
{
    private Address(
        string country,
        string? province,
        string? district,
        string? sector,
        string? cell,
        string? village,
        string? street)
    {
        Country = country;
        Province = province;
        District = district;
        Sector = sector;
        Cell = cell;
        Village = village;
        Street = street;
    }

    /// <summary>ISO 3166-1 alpha-2 country code, for example <c>RW</c>.</summary>
    public string Country { get; }

    public string? Province { get; }

    public string? District { get; }

    public string? Sector { get; }

    public string? Cell { get; }

    public string? Village { get; }

    public string? Street { get; }

    public static Address Create(
        string country,
        string? province = null,
        string? district = null,
        string? sector = null,
        string? cell = null,
        string? village = null,
        string? street = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(country);

        return new Address(
            country.Trim().ToUpperInvariant(),
            Normalise(province),
            Normalise(district),
            Normalise(sector),
            Normalise(cell),
            Normalise(village),
            Normalise(street));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Country;
        yield return Province;
        yield return District;
        yield return Sector;
        yield return Cell;
        yield return Village;
        yield return Street;
    }

    private static string? Normalise(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
