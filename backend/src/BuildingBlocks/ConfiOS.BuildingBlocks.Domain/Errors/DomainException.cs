namespace ConfiOS.BuildingBlocks.Domain.Errors;

/// <summary>
/// Thrown when a domain invariant would be broken. Carries a stable code so the API can
/// translate it without string matching.
/// </summary>
public class DomainException : Exception
{
    public DomainException(Error error)
        : base(error.Code) => Error = error;

    public DomainException(string code)
        : this(new Error(code))
    {
    }

    public DomainException(string code, Exception innerException)
        : base(code, innerException) => Error = new Error(code);

    public DomainException()
        : this(ErrorCodes.Unknown)
    {
    }

    public Error Error { get; } = new(ErrorCodes.Unknown);
}
