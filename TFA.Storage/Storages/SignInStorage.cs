using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain;
using TFA.Domain.Authentication;
using TFA.Domain.UseCases.SignIn;

namespace TFA.Storage.Storages;

internal class SignInStorage : ISignInStorage
{
    private readonly ForumDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IGuidFactory guidFactory;

    public SignInStorage(ForumDbContext dbContext, IMapper mapper, IGuidFactory guidFactory)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.guidFactory = guidFactory;
    }

    public Task<RecognisedUser?> FindUserAsync(string login, CancellationToken cancellationToken)
    {
        return dbContext.Users
            .Where(u => u.Login.Equals(login))
            .ProjectTo<RecognisedUser>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateSessionAsync(Guid userId, DateTimeOffset expirationMoment, CancellationToken cancellationToken)
    {
        var sessionId = guidFactory.Create();
        await dbContext.Sessions.AddAsync(new Entities.Session() { UserId = userId, SessionId = sessionId, ExpireAt = expirationMoment });
        await dbContext.SaveChangesAsync();
        return sessionId;
    }

}
