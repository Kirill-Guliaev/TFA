using TFA.Domain;
using TFA.Domain.UseCases.SignOn;
using TFA.Storage.Entities;

namespace TFA.Storage.Storages;

internal class SignOnStorage : ISignOnStorage
{
    private readonly ForumDbContext dbContext;
    private readonly IGuidFactory guidFactory;

    public SignOnStorage(
        ForumDbContext dbContext,
        IGuidFactory guidFactory)
    {
        this.dbContext = dbContext;
        this.guidFactory = guidFactory;
    }

    public async Task<Guid> CreateUser(string login, byte[] salt, byte[] hash, CancellationToken cancellationToken)
    {
        var userId = guidFactory.Create();
        await dbContext.Users.AddAsync(new User()
        {
            Login = login,
            Salt = salt,
            UserId = userId,
            PasswordHash = hash
        }, cancellationToken);
        await dbContext.SaveChangesAsync();
        return userId;
    }
}
