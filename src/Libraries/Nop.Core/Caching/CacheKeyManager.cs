using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Cache key manager
    /// </summary>
    /// <remarks>
    /// This class should be registered on IoC as singleton instance
    /// </remarks>
    public partial class CacheKeyManager : ICacheKeyManager
    {
        protected readonly ConcurrentDictionary<string, byte> _keys = new();

        /// <summary>
        /// Add the key
        /// </summary>
        /// <param name="key">The key to add</param>
        public void AddKey(string key)
        {
            _keys[key] = default;
        }

        /// <summary>
        /// Remove the key
        /// </summary>
        /// <param name="key">The key to remove</param>
        public void RemoveKey(string key)
        {
            _keys.Remove(key, out _);
        }
        
        /// <summary>
        /// Remove all keys
        /// </summary>
        public void Clear()
        {
            _keys.Clear();
        }

        /// <summary>
        /// Remove keys by prefix
        /// </summary>
        /// <param name="prefix">Prefix to delete keys</param>
        /// <returns>The list of removed keys</returns>
        public IEnumerable<string> RemoveByPrefix(string prefix)
        {
            foreach (var key in _keys.Keys.Where(k => k.StartsWith(prefix)))
            {
                _keys.Remove(key, out _);
                yield return key;
            }
        }

        /// <summary>
        /// The list of keys
        /// </summary>
        public IEnumerable<string> Keys => _keys.Keys;
    }
}
