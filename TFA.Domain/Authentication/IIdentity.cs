namespace TFA.Domain.Identity;

public interface IIdentity
{
    Guid UserId { get; }

    Guid SessionId { get; }
}

public static class IDentityExtensions
{
    public static bool IsAuthenticated(this IIdentity identity) => identity.UserId != Guid.Empty;
}
