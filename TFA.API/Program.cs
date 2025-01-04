using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TFA.API.Authentication;
using TFA.API.DependencyInjection;
using TFA.API.Middlewares;
using TFA.Domain.Authentication;
using TFA.Domain.DependencyInjection;
using TFA.Storage;
using TFA.Storage.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddForumDomain()
    .AddForumStorage(builder.Configuration.GetConnectionString("Postgres"));
builder.Services.AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<IAuthTokenStorage, AuthTokenStorage>();

builder.Services.AddApiLoggin(builder.Configuration, builder.Environment);
builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("Authentication").Bind);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var mapper = app.Services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();

app.Services.GetRequiredService<ForumDbContext>().Database.Migrate();
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program()
{
}