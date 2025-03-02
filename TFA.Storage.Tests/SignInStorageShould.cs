using FluentAssertions;
using TFA.Domain;
using TFA.Storage.Storages;

namespace TFA.Storage.Tests;

public class SignInStorageShould : IClassFixture<StorageTestFixture>, IAsyncLifetime
{
    private readonly StorageTestFixture fixture;
    private readonly SignInStorage sut;
    private Guid userId = Guid.Parse("0114e6c8-50da-4760-9de3-da2988703858");
    public SignInStorageShould(StorageTestFixture fixture, IGuidFactory guidFactory)
    {
        this.fixture = fixture;

        sut = new SignInStorage(fixture.GetDbContext(), fixture.GetMapper(), guidFactory);
    }

    public async Task DisposeAsync()
    {
        
    }

    public async Task InitializeAsync()
    {
        await using var dbContext = fixture.GetDbContext();
        await dbContext.Users.AddRangeAsync(
            new Entities.User { Login = "firstUser", UserId = userId, Salt = [1], PasswordHash = [2] },
            new Entities.User { Login = "secondUser", UserId = Guid.Parse("c69e5ebf-af2f-4384-b9fe-bb1b6f122825"), Salt = [3], PasswordHash = [4] }
        );
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task ReturnUser_WhenUserExist()
    {
        var actual = await sut.FindUserAsync("firstUser", CancellationToken.None);
        actual.Should().NotBeNull();
        actual!.UserId.Should().Be(userId);
    }

    //[Fact]
    //public async Task ReturnNull_WhenUserNotExist()
    //{
    //    var actual = await sut.FindUserAsync("notCorrectLogin", CancellationToken.None);
    //    actual.Should().BeNull();
    //}

}
