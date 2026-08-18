using System.Security.Cryptography;

namespace ProjectLoop.Documents.Core;

/// <summary>
/// Computes a streaming content hash used to record document integrity.
/// This does not deduplicate or persist anything — callers decide what to
/// do with the resulting hash.
/// </summary>
public static class DocumentContentHasher
{
    public static async Task<string> ComputeHashAsync(Stream content, CancellationToken cancellationToken = default)
    {
        var hashBytes = await SHA256.HashDataAsync(content, cancellationToken);
        return Convert.ToHexStringLower(hashBytes);
    }
}
