using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class InvitationTokenGeneratorTests
{
    [Fact]
    public void Generate_Produces_Distinct_Raw_Tokens()
    {
        var generator = new InvitationTokenGenerator();

        var first = generator.Generate();
        var second = generator.Generate();

        Assert.NotEqual(first.RawToken, second.RawToken);
        Assert.NotEqual(first.TokenHash, second.TokenHash);
    }

    [Fact]
    public void Generate_Produces_Hash_That_Does_Not_Contain_Raw_Token()
    {
        var generator = new InvitationTokenGenerator();

        var token = generator.Generate();

        Assert.DoesNotContain(token.RawToken, token.TokenHash, StringComparison.Ordinal);
    }

    [Fact]
    public void Hash_Is_Deterministic_For_The_Same_Raw_Token()
    {
        var generator = new InvitationTokenGenerator();
        var token = generator.Generate();

        var recomputedHash = generator.Hash(token.RawToken);

        Assert.Equal(token.TokenHash, recomputedHash);
    }

    [Fact]
    public void Hash_Differs_For_Different_Raw_Tokens()
    {
        var generator = new InvitationTokenGenerator();

        var hashA = generator.Hash("token-a");
        var hashB = generator.Hash("token-b");

        Assert.NotEqual(hashA, hashB);
    }
}
