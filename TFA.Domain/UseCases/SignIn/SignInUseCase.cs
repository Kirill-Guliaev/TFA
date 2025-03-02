using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using TFA.Domain.Authentication;
using TFA.Domain.Exceptions;
using TFA.Domain.Identity;

namespace TFA.Domain.UseCases.SignIn;

internal class SignInUseCase : ISignInUseCase
{
    private readonly IValidator<SignInCommand> validator;
    private readonly ISignInStorage storage;
    private readonly IPasswordManager passwordManager;
    private readonly ISymmetricEncryptor symmetricEncryptor;
    private readonly AuthenticationConfiguration configuration;

    public SignInUseCase(
        IValidator<SignInCommand> validator,
        ISignInStorage storage,
        IPasswordManager passwordManager,
        ISymmetricEncryptor symmetricEncryptor,
        IOptions<AuthenticationConfiguration> option)
    {
        this.validator = validator;
        this.storage = storage;
        this.passwordManager = passwordManager;
        this.symmetricEncryptor = symmetricEncryptor;
        configuration = option.Value;
    }

    public async Task<(IIdentity identity, string token)> ExecuteAsync(SignInCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var user = await storage.FindUserAsync(command.Login, cancellationToken);
        if (user is null)
        {
            throw new ValidationException(new ValidationFailure[]
            {
                new()
                {
                    PropertyName = nameof(command.Login),
                    ErrorCode = ValidationErrorCode.Invalid,
                    AttemptedValue = command.Login,
                }
            });
        }
        var passwordMatchs = passwordManager.ComparePasswords(command.Password, user.Salt, user.PasswordHash);
        if (!passwordMatchs)
        {
            throw new ValidationException(new ValidationFailure[]
            {
                new()
                {
                    PropertyName = nameof(command.Password),
                    ErrorCode = ValidationErrorCode.Invalid,
                    AttemptedValue = command.Password,
                }
            });
        }
        var sessionId = await storage.CreateSessionAsync(user.UserId, DateTimeOffset.Now + TimeSpan.FromHours(1), cancellationToken);
        var token = await symmetricEncryptor.EncryptAsync(sessionId.ToString(), configuration.Key, cancellationToken);
        return (new User(user.UserId, sessionId), token);
    }
}