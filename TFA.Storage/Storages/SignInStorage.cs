using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.Authentication;
using TFA.Domain.UseCases.SignIn;

namespace TFA.Storage.Storages;

internal class SignInStorage : ISignInStorage
{
    private readonly ForumDbContext dbContext;
    private readonly IMapper mapper;

    public SignInStorage(ForumDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public Task<RecognisedUser?> FindUserAsync(string login, CancellationToken cancellationToken)
    {
        return dbContext.Users
            .Where(u=>u.Login.Equals(login))
            .ProjectTo<RecognisedUser>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
