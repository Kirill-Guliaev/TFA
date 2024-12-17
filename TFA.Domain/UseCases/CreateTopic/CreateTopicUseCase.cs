using FluentValidation;
using TFA.Domain.Authorization;
using TFA.Domain.Identity;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Domain.UseCases.CreateTopic;

internal class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly IValidator<CreateTopicCommand> validator;
    private readonly IIntentionManager intentionManager;
    private readonly ICreateTopicStorage createTopicStorage;
    private readonly IGetForumsStorage getForumsStorage;
    private readonly IIdentityProvider identityProvider;

    public CreateTopicUseCase(
        IValidator<CreateTopicCommand> validator,
        IIntentionManager intentionManager,
        ICreateTopicStorage createTopicStorage,
        IGetForumsStorage getForumsStorage,
        IIdentityProvider identityProvider)
    {
        this.validator = validator;
        this.intentionManager = intentionManager;
        this.createTopicStorage = createTopicStorage;
        this.getForumsStorage = getForumsStorage;
        this.identityProvider = identityProvider;
    }

    public async Task<Topic> ExecuteAsync(CreateTopicCommand createTopicCommand, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(createTopicCommand, cancellationToken);
        var (forumId, title) = createTopicCommand;
        intentionManager.ThrowIfForbidden(TopicIntention.Create);
        await getForumsStorage.ThrowIfForumNotFount(forumId, cancellationToken);
        return await createTopicStorage.CreateTopicAsync(forumId, identityProvider.Current.UserId, title, cancellationToken);
    }
}
