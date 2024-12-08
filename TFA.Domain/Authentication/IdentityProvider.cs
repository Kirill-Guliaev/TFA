using TFA.Domain.Identity;

namespace TFA.Domain.Authentication;

internal class IdentityProvider : IIdentityProvider
{
    public IIdentity Current => new User(Guid.Parse("bd676dd8-16e3-4ec6-93c4-829f2b127e3c"));
}
