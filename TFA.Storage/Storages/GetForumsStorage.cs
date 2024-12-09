﻿using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Storage.Storages;

internal class GetForumsStorage : IGetForumsStorage
{
    private readonly ForumDbContext dbContext;

    public GetForumsStorage(ForumDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<IEnumerable<Domain.Models.Forum>> GetForumsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Forums.Select(f => new Domain.Models.Forum {  Id = f.ForumId, Title = f.Title }).ToArrayAsync(cancellationToken);
    }
}
