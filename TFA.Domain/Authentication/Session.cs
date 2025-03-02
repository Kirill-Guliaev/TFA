namespace TFA.Domain.Authentication;

public record Session(Guid UserId, DateTimeOffset ExpireAt);