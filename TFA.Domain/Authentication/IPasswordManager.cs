namespace TFA.Domain.Authentication;

internal interface IPasswordManager
{
    bool ComparePasswords(string password, byte[] salt, byte[] hash);

    (byte[] Salt, byte[] Hash) GeneratePasswordParts(string password);
}
