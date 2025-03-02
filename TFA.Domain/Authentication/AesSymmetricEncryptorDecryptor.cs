using System.Security.Cryptography;
using System.Text;

namespace TFA.Domain.Authentication;

internal class AesSymmetricEncryptorDecryptor : ISymmetricEncryptor, ISymmetricDecryptor
{
    private const int IvSize = 16;
    private readonly Lazy<Aes> aes = new(Aes.Create);

    public async Task<string> DecryptAsync(string encryptedText, byte[] key, CancellationToken cancellationToken)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedText);
        var iv = new byte[IvSize];
        Array.Copy(encryptedBytes, 0, iv, 0, IvSize);
        using var decryptedStream = new MemoryStream();
        await using (var stream = new CryptoStream(
           decryptedStream,
           aes.Value.CreateDecryptor(key, iv),
           CryptoStreamMode.Write))
        {
            await stream.WriteAsync(encryptedBytes.AsMemory(IvSize), cancellationToken);
        }
        return Encoding.UTF8.GetString(decryptedStream.ToArray());
    }

    public async Task<string> EncryptAsync(string text, byte[] key, CancellationToken cancellationToken)
    {
        var iv = RandomNumberGenerator.GetBytes(IvSize);
        using var encryptedSteam = new MemoryStream();
        await encryptedSteam.WriteAsync(iv, cancellationToken);
        var encryptor = aes.Value.CreateEncryptor(key, iv);
        await using (var stream = new CryptoStream(encryptedSteam, encryptor, CryptoStreamMode.Write))
        {
            await stream.WriteAsync(Encoding.UTF8.GetBytes(text), cancellationToken);
        }
        return Convert.ToBase64String(encryptedSteam.ToArray());
    }
}