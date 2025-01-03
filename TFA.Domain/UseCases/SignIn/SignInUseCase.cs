using FluentValidation;
using Microsoft.Extensions.Options;
using TFA.Domain.Authentication;
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
            throw new Exception();
        }
        var passwordMatchs = passwordManager.ComparePasswords(command.Password, user.Salt, user.PasswordHash);
        if (!passwordMatchs)
        {
            throw new Exception();
        }
        var token = await symmetricEncryptor.EncryptAsync(user.UserId.ToString(), configuration.Key, cancellationToken);
        return (new User(user.UserId), token);
    }
}