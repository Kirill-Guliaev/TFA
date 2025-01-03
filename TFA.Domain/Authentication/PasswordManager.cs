using System.Security.Cryptography;
using System.Text;

namespace TFA.Domain.Authentication;

internal class PasswordManager : IPasswordManager
{
    private const int SaltLength = 100;
    private readonly Lazy<SHA256> sha256 = new(SHA256.Create);

    public bool ComparePasswords(string password, byte[] salt, byte[] hash)
    {
        var computeHash = ComputeHash(password, salt);
        return computeHash.SequenceEqual(hash);
    }

    public (byte[] Salt, byte[] Hash) GeneratePasswordParts(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltLength);
        var hash = ComputeHash(password, salt);
        return (salt, hash.ToArray());
    }
    private ReadOnlySpan<byte> ComputeHash(string text, byte[] salt)
    {
        var textBytes = Encoding.UTF8.GetBytes(text);
        var buffer = new byte[textBytes.Length + salt.Length];
        Array.Copy(textBytes, buffer, textBytes.Length);
        Array.Copy(salt, 0, buffer, textBytes.Length, salt.Length);
        lock (sha256)
        {
            return sha256.Value.ComputeHash(buffer);
        }
    }
}