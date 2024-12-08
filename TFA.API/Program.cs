using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;
using TFA.API.Middlewares;
using TFA.Domain.DependencyInjection;
using TFA.Storage;
using TFA.Storage.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddForumDomain()
    .AddForumStorage(builder.Configuration.GetConnectionString("Postgres"));

builder.Services.AddLogging(b => b.AddSerilog(new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.WithProperty("Application", "TFA.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.OpenSearch(
            builder.Configuration.GetConnectionString("logs"),
            indexFormat: "forum-logs-{0:yyyy.MM.dd}"))
    .WriteTo.Logger(lc =>
        lc.Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.Console())
    .CreateLogger()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Services.GetRequiredService<ForumDbContext>().Database.Migrate();
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlerMiddleware>();

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
