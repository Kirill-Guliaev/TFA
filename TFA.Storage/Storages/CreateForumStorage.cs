﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Domain;
using TFA.Domain.Models;
using TFA.Domain.UseCases.CreateForum;

namespace TFA.Storage.Storages;

internal class CreateForumStorage : ICreateForumStorage
{
    private readonly IGuidFactory guidFactory;
    private readonly ForumDbContext forumDbContext;
    private readonly IMemoryCache memoryCache;

    public CreateForumStorage(IGuidFactory guidFactory,
        ForumDbContext forumDbContext,
        IMemoryCache memoryCache)
    {
        this.guidFactory = guidFactory;
        this.forumDbContext = forumDbContext;
        this.memoryCache = memoryCache;
    }

    public async Task<Domain.Models.Forum> Create(string Title, CancellationToken cancellationToken)
    {
        var newForum = new Forum
        {
            ForumId = guidFactory.Create(),
            Title = Title,
        };
        await forumDbContext.Forums.AddAsync(newForum, cancellationToken);
        await forumDbContext.SaveChangesAsync(cancellationToken);
        memoryCache.Remove(nameof(GetForumsStorage.GetForumsAsync));
        return await forumDbContext.Forums.Where(f => f.ForumId == newForum.ForumId)
            .Select(f => new Domain.Models.Forum
            {
                Id = f.ForumId,
                Title = f.Title
            })
            .FirstAsync(cancellationToken);
    }
}