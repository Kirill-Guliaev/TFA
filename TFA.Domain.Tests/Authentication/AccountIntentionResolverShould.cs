using FluentAssertions;
using Moq;
using TFA.Domain.Authentication;
using TFA.Domain.Identity;
using TFA.Domain.UseCases.SignOut;

namespace TFA.Domain.Tests.Authentication;

public class AccountIntentionResolverShould
{
    private readonly AccountIntentionResolver sut = new();

    [Fact]
    public void ReturnFalse_WhenIntentionNotEnum()
    {
        var intention = (AccountIntention)(-1);
        sut.IsAllowed(new Mock<IIdentity>().Object, intention).Should().BeFalse();
    }

    [Fact]
    public void ReturnFalse_WhenUserIsGuest()
    {
        sut.IsAllowed(User.Guest, AccountIntention.SignOut).Should().BeFalse(); 
    }

    [Fact]
    public void ReturnTrue_WhenUserAuthenticated()
    {
        sut.IsAllowed(new User(Guid.Parse("a7a9fa3e-73d6-41f6-b62a-5045adabb8c3"), Guid.Empty), AccountIntention.SignOut)
            .Should().BeTrue();    
    }
}
