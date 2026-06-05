namespace RepositoryManager;

/// <summary>
/// Holds the content and its type for a single stored item.
/// TContent is kept generic so that non-string content types can be supported
/// in the future without changing this class.
/// </summary>
public sealed class RepositoryItem<TContent>
{
    public TContent Content { get; }
    public int ItemType { get; }

    public RepositoryItem(TContent content, int itemType)
    {
        Content = content;
        ItemType = itemType;
    }
}
