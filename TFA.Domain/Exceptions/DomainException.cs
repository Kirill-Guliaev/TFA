namespace TFA.Domain.Exceptions;

public abstract class DomainException: Exception
{
    protected DomainException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
