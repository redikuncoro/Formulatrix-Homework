namespace RepositoryManager;

/// <summary>
/// Abstraction over the physical storage used by the repository.
/// Swap the implementation (in-memory, database, file-based, etc.)
/// without touching RepositoryManager itself.
/// </summary>
public interface IStorageBackend<TContent>
{
    /// <summary>Returns false when the key already exists (no overwrite).</summary>
    bool TryAdd(string key, RepositoryItem<TContent> item);

    bool TryGet(string key, out RepositoryItem<TContent>? item);

    bool TryRemove(string key);
}
