using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Identity;
using TFA.Domain.Models;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Domain.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddForumDomain(this IServiceCollection services)
    {
        services          
            .AddScoped<IGetForumsUseCase, GetForumsUseCase>()
            .AddScoped<ICreateTopicUseCase, CreateTopicUseCase>()
            .AddScoped<IIntentionResolver, TopicIntetionResolver>()
            .AddScoped<IIntentionManager, IntentionManager>()
            .AddScoped<IIdentityProvider, IdentityProvider>();
        services.AddValidatorsFromAssemblyContaining<Forum>(includeInternalTypes:true);
        return services;
    }
}
