﻿using Microsoft.EntityFrameworkCore;
using TFA.Storage.Entities;

namespace TFA.Storage;

public class ForumDbContext : DbContext
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Forum> Forums { get; set; } = null!;
    public DbSet<Topic> Topics { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
}
