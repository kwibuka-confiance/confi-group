using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ConfiOS.BuildingBlocks.Tests.ValueObjects;

public sealed class ContactValueObjectTests
{
    [Theory]
    [InlineData("+250788123456", "+250788123456")]
    [InlineData("+250 788 123 456", "+250788123456")]
    [InlineData("+250-788-123-456", "+250788123456")]
    public void Phone_numbers_are_normalised_to_e164(string input, string expected)
    {
        PhoneNumber.Create(input).Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("0788123456")]
    [InlineData("+0788123456")]
    [InlineData("not a number")]
    public void Phone_numbers_without_a_valid_country_code_are_rejected(string input)
    {
        Should.Throw<DomainException>(() => PhoneNumber.Create(input));
    }

    [Fact]
    public void Email_addresses_are_lower_cased()
    {
        EmailAddress.Create("  Owner@KwaConfi.RW ").Value.ShouldBe("owner@kwaconfi.rw");
    }

    [Theory]
    [InlineData("owner@")]
    [InlineData("@kwaconfi.rw")]
    [InlineData("owner@kwaconfi")]
    public void Malformed_email_addresses_are_rejected(string input)
    {
        Should.Throw<DomainException>(() => EmailAddress.Create(input));
    }
}
