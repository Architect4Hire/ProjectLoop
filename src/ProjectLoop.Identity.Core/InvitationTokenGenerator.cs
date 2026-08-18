using System.Security.Cryptography;

namespace ProjectLoop.Identity.Core;

public sealed class InvitationTokenGenerator : IInvitationTokenGenerator
{
    private const int RawTokenByteLength = 32;

    public InvitationToken Generate()
    {
        var rawToken = Base64Url(RandomNumberGenerator.GetBytes(RawTokenByteLength));
        return new InvitationToken(rawToken, Hash(rawToken));
    }

    public string Hash(string rawToken)
    {
        var hashBytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(hashBytes);
    }

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
