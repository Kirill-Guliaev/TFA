using TFA.Domain.Identity;

namespace TFA.Domain.Authentication;

public class User : IIdentity
{
    private readonly Guid userId;

    public User(Guid userId)
    {
        this.userId = userId;
    }

    public Guid UserId => userId;

    public static User Guest => new(Guid.Empty);
}
