using Nop.Core.Infrastructure;

namespace Nop.Core.Caching;

/// <summary>
/// Cache key manager
/// </summary>
/// <remarks>
/// This class should be registered on IoC as singleton instance
/// </remarks>
public partial class CacheKeyManager : ICacheKeyManager
{
    protected readonly IConcurrentCollection<byte> _keys;

    public CacheKeyManager(IConcurrentCollection<byte> keys)
    {
        _keys = keys;
    }

    /// <summary>
    /// Add the key
    /// </summary>
    /// <param name="key">The key to add</param>
    public virtual void AddKey(string key)
    {
        _keys.Add(key, default);
    }

    /// <summary>
    /// Remove the key
    /// </summary>
    /// <param name="key">The key to remove</param>
    public virtual void RemoveKey(string key)
    {
        _keys.Remove(key);
    }

    /// <summary>
    /// Remove all keys
    /// </summary>
    public virtual void Clear()
    {
        _keys.Clear();
    }

    /// <summary>
    /// Remove keys by prefix
    /// </summary>
    /// <param name="prefix">Prefix to delete keys</param>
    /// <returns>The list of removed keys</returns>
    public virtual IEnumerable<string> RemoveByPrefix(string prefix)
    {
        if (!_keys.Prune(prefix, out var subtree) || subtree?.Keys == null)
            return Enumerable.Empty<string>();

        return subtree.Keys;
    }

    /// <summary>
    /// The list of keys
    /// </summary>
    public IEnumerable<string> Keys => _keys.Keys;
}