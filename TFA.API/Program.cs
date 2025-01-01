using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TFA.API.DependencyInjection;
using TFA.API.Middlewares;
using TFA.Domain.DependencyInjection;
using TFA.Storage;
using TFA.Storage.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddForumDomain()
    .AddForumStorage(builder.Configuration.GetConnectionString("Postgres"));
builder.Services.AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));

builder.Services.AddApiLoggin(builder.Configuration, builder.Environment);

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

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program()
{
}