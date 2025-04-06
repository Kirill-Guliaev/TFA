using FluentAssertions;
using System.Net.Http.Json;
using TFA.Domain.Authentication;

namespace TFA.E2E;

public class AccountEndpointsShould : IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory factory;

    public AccountEndpointsShould(ForumApiApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task SignInAfterSignOn()
    {
        using var httpClien = factory.CreateClient();
        using var response = await httpClien.PostAsync("account", JsonContent.Create(new { login = "TestLogin", password = "Qwerty123" }));
        response.Invoking(r => r.EnsureSuccessStatusCode()).Should().NotThrow();
        var createdUser = await response.Content.ReadFromJsonAsync<User>();
        using var signInResponse = await httpClien.PostAsync("account/signin", JsonContent.Create(new { login = "TestLogin", password = "Qwerty123" }));
        signInResponse.IsSuccessStatusCode.Should().BeTrue();

        var signedInUser = await signInResponse.Content.ReadFromJsonAsync<User>();
        signedInUser!.UserId.Should().Be(createdUser!.UserId);

        var createForumResponse = await httpClien.PostAsync("forums", JsonContent.Create(new { title = "Test Title" }));
        createForumResponse.IsSuccessStatusCode.Should().BeTrue();
    }
}
