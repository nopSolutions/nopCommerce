using System.Collections.Concurrent;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// A thread-safe implementation of a trie, or prefix tree
    /// </summary>
    public partial class ConcurrentTrie<TValue>
    {
        #region Fields

        protected readonly TrieNode _root;
        protected readonly string _prefix;

        #endregion

        #region Ctor

        public ConcurrentTrie() : this(new(), string.Empty)
        {
        }

        protected ConcurrentTrie(TrieNode root, string prefix)
        {
            _root = root;
            _prefix = prefix;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Adds a node if key does not already exist.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The <see cref="TrieNode"/> item</returns>
        protected virtual TrieNode GetOrAddNode(string key)
        {
            var node = _root;

            foreach (var c in key)
                node = node.Children.GetOrAdd(c, _ => new());

            return node;
        }

        /// <summary>
        /// Try to find the node by key
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="node">The <see cref="TrieNode"/> item</param>
        /// <returns>True if item is exists</returns>
        protected virtual bool Find(string key, out TrieNode node)
        {
            node = _root;

            foreach (var c in key)
                if (!node.Children.TryGetValue(c, out node))
                    return false;

            return true;
        }

        /// <summary>
        /// Remove the node
        /// </summary>
        /// <param name="node">The <see cref="TrieNode"/> item</param>
        /// <param name="key">The key</param>
        /// <returns>True if item is deleted</returns>
        protected virtual bool Remove(TrieNode node, string key)
        {
            if (key.Length == 0)
            {
                if (node.GetValue(out _))
                    node.RemoveValue();

                return !node.Children.IsEmpty;
            }

            var c = key[0];

            if (node.Children.TryGetValue(c, out var child))
                if (!Remove(child, key[1..]))
                    node.Children.TryRemove(new(c, child));

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to get the value associated with the specified key
        /// </summary>
        /// <param name="key">The key of the item to get (case-insensitive)</param>
        /// <param name="value">The value associated with <paramref name="key"/>, if found</param>
        /// <returns>
        /// True if the key was found, otherwise false
        /// </returns>
        public virtual bool TryGetValue(string key, out TValue value)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            value = default;
            return Find(key, out var node) && node.GetValue(out value);
        }

        /// <summary>
        /// Adds a key-value pair to the trie
        /// </summary>
        /// <param name="key">The key of the new item (case-insensitive)</param>
        /// <param name="value">The value to be associated with <paramref name="key"/></param>
        public virtual void Add(string key, TValue value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));

            GetOrAddNode(key).SetValue(value);
        }

        /// <summary>
        /// Clears the trie
        /// </summary>
        public virtual void Clear()
        {
            _root.Children.Clear();
        }

        /// <summary>
        /// Gets all key-value pairs for keys starting with the given prefix
        /// </summary>
        /// <param name="prefix">The prefix to search for (case-insensitive)</param>
        /// <returns>
        /// All key-value pairs for keys starting with <paramref name="prefix"/>
        /// </returns>
        public virtual IEnumerable<KeyValuePair<string, TValue>> Search(string prefix)
        {
            if (prefix is null)
                throw new ArgumentNullException(nameof(prefix));

            if (!Find(prefix, out var node))
                return Enumerable.Empty<KeyValuePair<string, TValue>>();

            // depth-first traversal
            IEnumerable<KeyValuePair<string, TValue>> traverse(TrieNode n, string s)
            {
                if (n.GetValue(out var value))
                    yield return new KeyValuePair<string, TValue>(_prefix + s, value);
                foreach (var (c, child) in n.Children)
                {
                    foreach (var kv in traverse(child, s + c))
                        yield return kv;
                }
            }
            return traverse(node, prefix);
        }

        /// <summary>
        /// Removes the item with the given key, if present
        /// </summary>
        /// <param name="key">The key of the item to be removed (case-insensitive)</param>
        public virtual void Remove(string key)
        {
            Remove(_root, key);
        }

        /// <summary>
        /// Gets the value with the specified key, adding a new item if one does not exist
        /// </summary>
        /// <param name="key">The key of the item to be deleted (case-insensitive)</param>
        /// <param name="valueFactory">A function for producing a new value if one was not found</param>
        /// <returns>
        /// The existing value for the given key, if found, otherwise the newly inserted value
        /// </returns>
        public virtual TValue GetOrAdd(string key, Func<TValue> valueFactory)
        {
            var node = GetOrAddNode(key);
            if (node.GetValue(out var value))
                return value;
            value = valueFactory();
            node.SetValue(value);
            return value;
        }

        /// <summary>
        /// Attempts to remove all items with keys starting with the specified prefix
        /// </summary>
        /// <param name="prefix">The prefix of the items to be deleted (case-insensitive)</param>
        /// <param name="subtree">The subtree containing all deleted items, if found</param>
        /// <returns>
        /// True if the prefix was successfully removed from the trie, otherwise false
        /// </returns>
        public virtual bool Prune(string prefix, out ConcurrentTrie<TValue> subtree)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException($"'{nameof(prefix)}' cannot be null or empty.", nameof(prefix));

            subtree = default;
            var node = _root;
            TrieNode parent = null;
            char last = default;
            foreach (var c in prefix)
            {
                parent = node;
                if (!node.Children.TryGetValue(c, out node))
                    return false;
                last = c;
            }
            if (parent?.Children.TryRemove(last, out var subtreeRoot) == true)
                subtree = new ConcurrentTrie<TValue>(subtreeRoot, prefix);
            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Keys
        /// </summary>
        public virtual IEnumerable<string> Keys => Search(string.Empty).Select(kv => kv.Key);

        /// <summary>
        /// Values
        /// </summary>
        public virtual IEnumerable<TValue> Values => Search(string.Empty).Select(kv => kv.Value);

        #endregion

        #region Nested class

        /// <summary>
        /// A thread-safe implementation of a trie node
        /// </summary>
        protected class TrieNode
        {
            #region Fields

            protected readonly ReaderWriterLockSlim _lock = new();
            protected (bool hasValue, TValue value) _value;

            #endregion

            #region Utilities

            protected virtual void SetValue(TValue value, bool hasValue)
            {
                _lock.EnterWriteLock();

                try
                {
                    _value = (hasValue, value);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }

            #endregion

            #region Methods

            public virtual bool GetValue(out TValue value)
            {
                _lock.EnterReadLock();
                try
                {
                    (var hasValue, value) = _value;

                    return hasValue;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }

            public virtual void SetValue(TValue value)
            {
                SetValue(value, true);
            }

            public virtual void RemoveValue()
            {
                SetValue(default, false);
            }

            #endregion

            #region Properties

            public readonly ConcurrentDictionary<char, TrieNode> Children = new();

            #endregion
        }

        #endregion
    }
}
