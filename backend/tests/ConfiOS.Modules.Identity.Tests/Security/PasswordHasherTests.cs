using ConfiOS.Modules.Identity.Infrastructure.Security;
using Shouldly;
using Xunit;

namespace ConfiOS.Modules.Identity.Tests.Security;

public sealed class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void The_same_password_hashes_differently_each_time()
    {
        // Distinct salts, so identical passwords do not produce identical stored values.
        _hasher.Hash("correct horse battery staple")
            .ShouldNotBe(_hasher.Hash("correct horse battery staple"));
    }

    [Fact]
    public void A_correct_password_verifies()
    {
        var hash = _hasher.Hash("correct horse battery staple");

        _hasher.Verify("correct horse battery staple", hash).ShouldBeTrue();
    }

    [Fact]
    public void An_incorrect_password_does_not_verify()
    {
        var hash = _hasher.Hash("correct horse battery staple");

        _hasher.Verify("Correct horse battery staple", hash).ShouldBeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-valid-format")]
    [InlineData("v9.1000.aaaa.bbbb")]
    public void A_malformed_stored_hash_returns_false_rather_than_throwing(string hash)
    {
        _hasher.Verify("any password", hash).ShouldBeFalse();
    }

    [Fact]
    public void The_stored_format_carries_its_version_and_work_factor()
    {
        var parts = _hasher.Hash("correct horse battery staple").Split('.');

        parts.Length.ShouldBe(4);
        parts[0].ShouldBe("v1");
        int.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture).ShouldBeGreaterThan(100_000);
    }
}
