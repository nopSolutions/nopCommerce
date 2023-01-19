using System.Collections.Generic;
using System.Linq;
using Nop.Core.Infrastructure;

namespace Nop.Core.Caching
{
    public class CacheKeyManager
    {
        private readonly ConcurrentTrie<byte> _keys = new();

        public void AddKey(string key)
        {
            _keys.Add(key, default);
        }

        public void RemoveKey(string key)
        {
            _keys.Remove(key);
        }

        public void Clear()
        {
            _keys.Clear();
        }

        public IEnumerable<string> RemoveByPrefix(string prefix)
        {
            return _keys.Prune(prefix, out var subtree)
                ? subtree.Keys
                : Enumerable.Empty<string>();
        }

        public IEnumerable<string> Keys => _keys.Keys;
    }
}
