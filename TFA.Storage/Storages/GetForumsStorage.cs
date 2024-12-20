﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Storage.Storages;

internal class GetForumsStorage : IGetForumsStorage
{
    private readonly IMemoryCache memoryCache;
    private readonly ForumDbContext dbContext;

    public GetForumsStorage(
        IMemoryCache memoryCache,
        ForumDbContext dbContext)
    {
        this.memoryCache = memoryCache;
        this.dbContext = dbContext;
    }
    public async Task<IEnumerable<Domain.Models.Forum>> GetForumsAsync(CancellationToken cancellationToken)
    {
        return await memoryCache.GetOrCreateAsync<Domain.Models.Forum[]>(
            key: nameof(GetForumsAsync),
            async entry => {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                return await dbContext.Forums
                .AsNoTrackingWithIdentityResolution()
                .Select(f => new Domain.Models.Forum { Id = f.ForumId, Title = f.Title })
                .ToArrayAsync(cancellationToken);
                }
        );

    }
}
