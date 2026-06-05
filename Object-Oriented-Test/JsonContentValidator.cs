using System.Text.Json;

namespace RepositoryManager;

public sealed class JsonContentValidator : IContentValidator
{
    public bool Validate(string content)
    {
        try
        {
            using var doc = JsonDocument.Parse(content);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
