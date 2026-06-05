namespace RepositoryManager;

/// <summary>
/// Validates string content for a specific item type (e.g. JSON, XML).
/// </summary>
public interface IContentValidator
{
    bool Validate(string content);
}
