namespace ProjectLoop.Documents.Core;

/// <summary>
/// Validates and normalizes a client-supplied display file name before it is
/// used anywhere downstream. Defends against path traversal and control
/// characters; the normalized value is never treated as a storage path — the
/// Blob object key remains a service-generated opaque identifier.
/// </summary>
public static class DocumentFileNamePolicy
{
    private const int MaxLength = 256;

    private static readonly char[] PathSeparators = ['\\', '/'];

    public static bool TryNormalize(string? fileName, out string normalized)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        // Strip any directory component; only the leaf name is ever
        // considered, defeating "../" and absolute-path traversal attempts.
        var leaf = fileName;
        var lastSeparatorIndex = leaf.LastIndexOfAny(PathSeparators);
        if (lastSeparatorIndex >= 0)
        {
            leaf = leaf[(lastSeparatorIndex + 1)..];
        }

        leaf = leaf.Trim();

        if (leaf.Length == 0 || leaf.Length > MaxLength)
        {
            return false;
        }

        if (leaf is "." or "..")
        {
            return false;
        }

        if (leaf.Any(char.IsControl))
        {
            return false;
        }

        normalized = leaf;
        return true;
    }
}
