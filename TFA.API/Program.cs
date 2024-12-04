using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TFA.API.Middlewares;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Identity;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Storage;
using TFA.Storage.Storages;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres");

builder.Services.AddScoped<IGetForumsUseCase, GetForumsUseCase>();
builder.Services.AddScoped<ICreateTopicUseCase, CreateTopicUseCase>();
builder.Services.AddScoped<ICreateTopicStorage, CreateTopicStorage>();
builder.Services.AddScoped<IGetForumsStorage, GetForumsStorage>();
builder.Services.AddScoped<IIntentionResolver, TopicIntetionResolver>();
builder.Services.AddScoped<IIntentionManager, IntentionManager>();
builder.Services.AddScoped<IIdentityProvider, IdentityProvider>();
builder.Services.AddScoped<IIdentityProvider, IdentityProvider>();
builder.Services.AddValidatorsFromAssemblyContaining<TFA.Domain.Models.Forum>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ForumDbContext>(options => options.UseNpgsql(connectionString));
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlerMiddleware>();

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
