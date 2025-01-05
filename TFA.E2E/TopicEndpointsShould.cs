using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;

namespace TFA.E2E;

public class TopicEndpointsShould : IClassFixture<ForumApiApplicationFactory>, IAsyncLifetime
{
    private readonly ForumApiApplicationFactory factory;
    public TopicEndpointsShould(ForumApiApplicationFactory factory)
    {
        this.factory = factory;
    }
    public async Task InitializeAsync()
    {
        //var dbContext = factory.Services.GetRequiredService<ForumDbContext>();
        //var cretedForum = await dbContext.Forums.AddAsync(
        //    new Forum()
        //    {
        //        ForumId = forumId,
        //        Title = "Test forum"
        //    }
        //    );
        //await dbContext.SaveChangesAsync();
    }

    //[Fact]
    public async Task ReturnForbidden_WhenNotAutenticated()
    {
        using var httpClient = factory.CreateClient();
        using var forumCreatedResponse = await httpClient.PostAsync("forums", JsonContent.Create(new { Title = "Test forum" }));
        forumCreatedResponse.EnsureSuccessStatusCode();
        var createdForum = await forumCreatedResponse.Content.ReadFromJsonAsync<API.Models.Forum>();
        createdForum.Should().NotBeNull();

        using var response = await httpClient.PostAsync($"forums/{createdForum!.Id}/topics", 
            JsonContent.Create(new { Title = "Test topic name" }));
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

}
