using System.Security.Cryptography;
using System.Text;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentContentHasherTests
{
    [Fact]
    public async Task ComputeHashAsync_Returns_Lowercase_Sha256_Hex_Digest()
    {
        var payload = "document-content"u8.ToArray();
        var expected = Convert.ToHexStringLower(SHA256.HashData(payload));

        using var stream = new MemoryStream(payload);
        var hash = await DocumentContentHasher.ComputeHashAsync(stream);

        Assert.Equal(expected, hash);
        Assert.Equal(64, hash.Length);
    }

    [Fact]
    public async Task ComputeHashAsync_Produces_Different_Hashes_For_Different_Content()
    {
        using var streamA = new MemoryStream(Encoding.UTF8.GetBytes("content-a"));
        using var streamB = new MemoryStream(Encoding.UTF8.GetBytes("content-b"));

        var hashA = await DocumentContentHasher.ComputeHashAsync(streamA);
        var hashB = await DocumentContentHasher.ComputeHashAsync(streamB);

        Assert.NotEqual(hashA, hashB);
    }
}
