namespace TFA.Domain.Authentication;

internal interface ISymmetricEncryptor
{
    Task<string> EncryptAsync(string text, byte[] key, CancellationToken cancellationToken);
}
