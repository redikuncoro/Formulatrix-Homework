using System;
using System.Collections.Generic;

namespace RepositoryManager;

/// <summary>
/// Repository manager that stores and retrieves string content (JSON or XML)
/// identified by a unique name.
///
/// Thread safety: all public methods are safe to call concurrently.
///   - Overwrite protection and duplicate detection rely on the atomic
///     TryAdd semantics of the underlying IStorageBackend.
///   - Initialize() uses a lock to guarantee it is executed at most once.
///
/// Extensibility:
///   - Inject a custom IStorageBackend&lt;string&gt; to swap the persistence layer
///     (database, file-based, etc.) without changing this class.
///   - The generic RepositoryItem&lt;TContent&gt; and IStorageBackend&lt;TContent&gt;
///     allow non-string content types to be supported by creating a
///     specialised manager or subclass.
/// </summary>
public sealed class RepositoryManager
{
    private readonly IStorageBackend<string> _storage;
    private readonly IReadOnlyDictionary<int, IContentValidator> _validators;

    private bool _initialized;
    private readonly object _initLock = new();

    // -----------------------------------------------------------------------
    // Construction
    // -----------------------------------------------------------------------

    /// <summary>Creates a manager backed by the default in-memory storage.</summary>
    public RepositoryManager()
        : this(new InMemoryStorageBackend<string>()) { }

    /// <summary>
    /// Creates a manager with a custom storage backend.
    /// Useful for testing (mock storage) or production (DB / file backend).
    /// </summary>
    public RepositoryManager(IStorageBackend<string> storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _validators = new Dictionary<int, IContentValidator>
        {
            { (int)ItemType.Json, new JsonContentValidator() },
            { (int)ItemType.Xml,  new XmlContentValidator()  }
        };
    }

    // -----------------------------------------------------------------------
    // Public API (as specified)
    // -----------------------------------------------------------------------

    /// <summary>
    /// Initializes the repository for use.
    /// Must be called at most once after the instance is created.
    /// Calling it a second time throws InvalidOperationException.
    /// </summary>
    public void Initialize()
    {
        lock (_initLock)
        {
            if (_initialized)
                throw new InvalidOperationException(
                    "Initialize() has already been called on this instance.");
            _initialized = true;
        }
    }

    /// <summary>
    /// Stores an item in the repository.
    /// itemType: 1 = JSON, 2 = XML.
    /// Throws if the name is already registered (no overwrite).
    /// Throws if itemContent fails validation for the given itemType.
    /// </summary>
    public void Register(string itemName, string itemContent, int itemType)
    {
        if (string.IsNullOrEmpty(itemName))
            throw new ArgumentException("Item name cannot be null or empty.", nameof(itemName));

        if (itemContent is null)
            throw new ArgumentNullException(nameof(itemContent));

        if (!_validators.TryGetValue(itemType, out var validator))
            throw new ArgumentException(
                $"Unknown item type '{itemType}'. Valid values: 1 (JSON), 2 (XML).",
                nameof(itemType));

        if (!validator.Validate(itemContent))
            throw new ArgumentException(
                $"Item content is not valid for item type {itemType}.",
                nameof(itemContent));

        if (!_storage.TryAdd(itemName, new RepositoryItem<string>(itemContent, itemType)))
            throw new InvalidOperationException(
                $"An item named '{itemName}' is already registered and cannot be overwritten.");
    }

    /// <summary>Retrieves the stored content for the given item name.</summary>
    public string Retrieve(string itemName)
    {
        if (!_storage.TryGet(itemName, out var item) || item is null)
            throw new KeyNotFoundException($"No item found with the name '{itemName}'.");

        return item.Content;
    }

    /// <summary>Returns the type (1 = JSON, 2 = XML) of the stored item.</summary>
    public int GetType(string itemName)
    {
        if (!_storage.TryGet(itemName, out var item) || item is null)
            throw new KeyNotFoundException($"No item found with the name '{itemName}'.");

        return item.ItemType;
    }

    /// <summary>Removes the item with the given name from the repository.</summary>
    public void Deregister(string itemName)
    {
        if (!_storage.TryRemove(itemName))
            throw new KeyNotFoundException($"No item found with the name '{itemName}'.");
    }
}
