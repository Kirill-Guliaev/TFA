using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.Authentication;

namespace TFA.Storage.Storages;

public class AuthenticationStorage : IAuthenticationStorage
{
    private readonly ForumDbContext forumDbContext;
    private readonly IMapper mapper;

    public AuthenticationStorage(ForumDbContext forumDbContext, IMapper mapper)
    {
        this.forumDbContext = forumDbContext;
        this.mapper = mapper;
    }

    public async Task<Session?> FindSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        return await forumDbContext.Sessions
                .Where(s => s.SessionId == sessionId)
                .ProjectTo<Session>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
