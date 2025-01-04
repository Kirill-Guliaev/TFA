using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain;
using TFA.Storage.Storages;

namespace TFA.Storage.Tests;

public class CreateForumStorageShould : IClassFixture<StorageTestFixture>
{
    private readonly CreateForumStorage sut;
    private readonly StorageTestFixture fixture;

    public CreateForumStorageShould(StorageTestFixture fixture)
    {
        sut = new CreateForumStorage(new GuidFactory(), fixture.GetDbContext(), fixture.GetMemoryCache(), fixture.GetMapper());
        this.fixture = fixture;
    }
    [Fact]

    public async Task InsertNewForumInDatabase()
    {
        var forum = await sut.CreateAsync("Test title", CancellationToken.None);
        forum.Id.Should().NotBeEmpty();
        await using var dbContext = fixture.GetDbContext();
        var forumTitles = await dbContext.Forums
            .Where(f => f.ForumId == forum.Id)
            .Select(f => f.Title).ToArrayAsync();
        forumTitles.Should().HaveCount(1).And.Contain("Test title");

    }
}
