using System.Collections.Concurrent;

namespace RepositoryManager;

/// <summary>
/// Thread-safe in-memory implementation of IStorageBackend backed by a
/// ConcurrentDictionary. Replace this class with a database or file-based
/// implementation to change the persistence strategy without touching the
/// rest of the library.
/// </summary>
public sealed class InMemoryStorageBackend<TContent> : IStorageBackend<TContent>
{
    private readonly ConcurrentDictionary<string, RepositoryItem<TContent>> _store = new();

    public bool TryAdd(string key, RepositoryItem<TContent> item)
        => _store.TryAdd(key, item);

    public bool TryGet(string key, out RepositoryItem<TContent>? item)
        => _store.TryGetValue(key, out item);

    public bool TryRemove(string key)
        => _store.TryRemove(key, out _);
}
