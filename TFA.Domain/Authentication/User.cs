using TFA.Domain.Identity;

namespace TFA.Domain.Authentication;

public class User : IIdentity
{
    private readonly Guid userId;
    private readonly Guid sessionId;

    public User(Guid userId, Guid sessionId)
    {
        this.userId = userId;
        this.sessionId = sessionId;
    }

    public Guid UserId => userId;

    public Guid SessionId => sessionId;

    public static User Guest => new(Guid.Empty, Guid.Empty);
}
