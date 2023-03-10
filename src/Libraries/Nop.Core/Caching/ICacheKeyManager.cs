namespace Nop.Core.Caching
{
    /// <summary>
    /// Represents a cache key manager
    /// </summary>
    public interface ICacheKeyManager
    {
        /// <summary>
        /// Add the key
        /// </summary>
        /// <param name="key">The key to add</param>
        void AddKey(string key);

        /// <summary>
        /// Remove the key
        /// </summary>
        /// <param name="key">The key to remove</param>
        void RemoveKey(string key);

        /// <summary>
        /// Remove all keys
        /// </summary>
        void Clear();

        /// <summary>
        /// Remove keys by prefix
        /// </summary>
        /// <param name="prefix">Prefix to delete keys</param>
        /// <returns>The list of removed keys</returns>
        IEnumerable<string> RemoveByPrefix(string prefix);

        /// <summary>
        /// The list of keys
        /// </summary>
        IEnumerable<string> Keys { get; }
    }
}