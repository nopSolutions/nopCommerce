using System.Runtime.CompilerServices;

namespace Nop.Core.Infrastructure;

/// <summary>
/// A thread-safe implementation of a radix tree
/// </summary>
public partial class ConcurrentTrie<TValue> : IConcurrentCollection<TValue>
{
    #region Fields

    protected volatile TrieNode _root = new();
    protected readonly StripedReaderWriterLock _locks = new();
    protected readonly ReaderWriterLockSlim _structureLock = new();

    #endregion

    #region Ctor

    /// <summary>
    /// Initializes a new instance of <see cref="ConcurrentTrie{TValue}" />
    /// </summary>
    protected ConcurrentTrie(TrieNode subtreeRoot)
    {
        _root.Children[subtreeRoot.Label[0]] = subtreeRoot;
    }

    /// <summary>
    /// Initializes a new empty instance of <see cref="ConcurrentTrie{TValue}" />
    /// </summary>
    public ConcurrentTrie()
    {
    }

    #endregion

    #region Utilities

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int GetCommonPrefixLength(ReadOnlySpan<char> s1, ReadOnlySpan<char> s2)
    {
        var i = 0;
        var minLength = Math.Min(s1.Length, s2.Length);

        while (i < minLength && s2[i] == s1[i])
            i++;

        return i;
    }

    /// <summary>
    /// Gets a lock on the node's children
    /// </summary>
    /// <remarks>
    /// May return the same lock for two different nodes, so the user needs to check to avoid lock recursion exceptions
    /// </remarks>
    protected virtual ReaderWriterLockSlim GetLock(TrieNode node)
    {
        return _locks.GetLock(node.Children);
    }

    protected virtual bool Find(string key, TrieNode subtreeRoot, out TrieNode node)
    {
        node = subtreeRoot;

        if (key.Length == 0)
            return true;

        var suffix = key.AsSpan();

        while (true)
        {
            var nodeLock = GetLock(node);
            nodeLock.EnterReadLock();

            try
            {
                if (!node.Children.TryGetValue(suffix[0], out node))
                    return false;
            }
            finally
            {
                nodeLock.ExitReadLock();
            }

            var span = node.Label.AsSpan();
            var i = GetCommonPrefixLength(suffix, span);

            if (i != span.Length)
                return false;

            if (i == suffix.Length)
                return node.HasValue;

            suffix = suffix[i..];
        }
    }

    protected virtual TrieNode GetOrAddNode(ReadOnlySpan<char> key, TValue value, bool overwrite = false)
    {
        var node = _root;
        var suffix = key;
        ReaderWriterLockSlim nodeLock;
        char c;
        TrieNode nextNode;
        _structureLock.EnterReadLock();

        try
        {
            while (true)
            {
                c = suffix[0];
                nodeLock = GetLock(node);
                nodeLock.EnterUpgradeableReadLock();

                try
                {
                    if (node.Children.TryGetValue(c, out nextNode))
                    {
                        var label = nextNode.Label.AsSpan();
                        var i = GetCommonPrefixLength(label, suffix);
                        // suffix starts with label
                        if (i == label.Length)
                        {
                            // keys are equal - this is the node we're looking for
                            if (i == suffix.Length)
                            {
                                if (overwrite)
                                    nextNode.SetValue(value);
                                else
                                    nextNode.GetOrAddValue(value);

                                return nextNode;
                            }

                            // advance the suffix and continue the search from nextNode
                            suffix = suffix[label.Length..];
                            node = nextNode;

                            continue;
                        }

                        // we need to add a node, but don't want to hold an upgradeable read lock on _structureLock
                        // since only one can be held at a time, so we break, release the lock and reacquire a write lock
                        break;
                    }

                    // if there is no child starting with c, we can just add and return one
                    nodeLock.EnterWriteLock();

                    try
                    {
                        var suffixNode = new TrieNode(suffix);
                        suffixNode.SetValue(value);

                        return node.Children[c] = suffixNode;
                    }
                    finally
                    {
                        nodeLock.ExitWriteLock();
                    }
                }
                finally
                {
                    nodeLock.ExitUpgradeableReadLock();
                }
            }
        }
        finally
        {
            _structureLock.ExitReadLock();
        }

        // if we need to restructure the tree, we do it after releasing and reacquiring the lock.
        // however, another thread may have restructured around the node we're on in the meantime,
        // and in that case we need to retry the insertion
        _structureLock.EnterWriteLock();
        nodeLock.EnterUpgradeableReadLock();

        try
        {
            // we use while instead of if so we can break
            // and put a recursive call at the end of the method to enable tail-recursion optimization
            while (!node.IsDeleted && node.Children.TryGetValue(c, out nextNode))
            {
                var label = nextNode.Label.AsSpan();
                var i = GetCommonPrefixLength(label, suffix);

                // suffix starts with label?
                if (i == label.Length)
                {
                    // if the keys are equal, the key has already been inserted
                    if (i == suffix.Length)
                    {
                        if (overwrite)
                            nextNode.SetValue(value);

                        return nextNode;
                    }

                    // structure has changed since last; try again
                    break;
                }

                var splitNode = new TrieNode(suffix[..i])
                {
                    Children = { [label[i]] = new TrieNode(label[i..], nextNode) }
                };

                TrieNode outNode;

                // label starts with suffix, so we can return splitNode
                if (i == suffix.Length)
                    outNode = splitNode;
                // the keys diverge, so we need to branch from splitNode
                else
                    splitNode.Children[suffix[i]] = outNode = new TrieNode(suffix[i..]);

                outNode.SetValue(value);
                nodeLock.EnterWriteLock();

                try
                {
                    node.Children[c] = splitNode;
                }
                finally
                {
                    nodeLock.ExitWriteLock();
                }

                return outNode;
            }
        }
        finally
        {
            nodeLock.ExitUpgradeableReadLock();
            _structureLock.ExitWriteLock();
        }

        // we failed to add a node, so we have to retry;
        // the recursive call is placed at the end to enable tail-recursion optimization
        return GetOrAddNode(key, value, overwrite);
    }

    protected virtual void Remove(TrieNode subtreeRoot, ReadOnlySpan<char> key)
    {
        TrieNode node = null, grandparent = null;
        var parent = subtreeRoot;
        var i = 0;
        _structureLock.EnterReadLock();
        try
        {
            while (i < key.Length)
            {
                var c = key[i];
                var parentLock = GetLock(parent);
                parentLock.EnterReadLock();

                try
                {
                    if (!parent.Children.TryGetValue(c, out node))
                        return;
                }
                finally
                {
                    parentLock.ExitReadLock();
                }

                var label = node.Label.AsSpan();
                var k = GetCommonPrefixLength(key[i..], label);

                // is this the node we're looking for?
                if (k == label.Length && k == key.Length - i)
                {
                    // this node has to be removed or merged
                    if (node.TryRemoveValue(out _))
                        break;

                    // the node is either already removed, or it is a branching node
                    return;
                }

                if (k < label.Length)
                    return;

                i += label.Length;
                grandparent = parent;
                parent = node;
            }
        }
        finally
        {
            _structureLock.ExitReadLock();
        }

        if (node == null)
            return;

        // if we need to delete a node, the tree has to be restructured to remove empty leaves or merge
        // single children with branching node parents, and other threads may be currently on these nodes
        _structureLock.EnterWriteLock();

        try
        {
            var nodeLock = GetLock(node);
            var parentLock = GetLock(parent);
            var grandparentLock = grandparent != null ? GetLock(grandparent) : null;
            var lockAlreadyHeld = nodeLock == parentLock || nodeLock == grandparentLock;

            if (lockAlreadyHeld)
                nodeLock.EnterUpgradeableReadLock();
            else
                nodeLock.EnterReadLock();

            try
            {
                // another thread has written a value to the node while we were waiting
                if (node.HasValue)
                    return;

                var c = node.Label[0];
                var nChildren = node.Children.Count;

                // if the node has no children, we can just remove it
                if (nChildren == 0)
                {
                    parentLock.EnterWriteLock();
                    try
                    {
                        // was removed or replaced by another thread
                        if (!parent.Children.TryGetValue(c, out var n) || n != node)
                            return;

                        parent.Children.Remove(c);
                        node.Delete();

                        // since we removed a node, we may be able to merge a lone sibling with the parent
                        if (parent.Children.Count == 1 && grandparent != null && !parent.HasValue)
                        {
                            var grandparentLockAlreadyHeld = grandparentLock == parentLock;

                            if (!grandparentLockAlreadyHeld)
                                grandparentLock.EnterWriteLock();

                            try
                            {
                                c = parent.Label[0];

                                if (!grandparent.Children.TryGetValue(c, out n) || n != parent || parent.HasValue)
                                    return;

                                var child = parent.Children.First().Value;
                                grandparent.Children[c] = new TrieNode(parent.Label + child.Label, child);
                                parent.Delete();
                            }
                            finally
                            {
                                if (!grandparentLockAlreadyHeld)
                                    grandparentLock.ExitWriteLock();
                            }
                        }
                    }
                    finally
                    {
                        parentLock.ExitWriteLock();
                    }
                }
                // if there is a single child, we can merge it with node
                else if (nChildren == 1)
                {
                    parentLock.EnterWriteLock();

                    try
                    {
                        // was removed or replaced by another thread
                        if (!parent.Children.TryGetValue(c, out var n) || n != node)
                            return;

                        var child = node.Children.FirstOrDefault().Value;
                        parent.Children[c] = new TrieNode(node.Label + child.Label, child);
                        node.Delete();
                    }
                    finally
                    {
                        parentLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                if (lockAlreadyHeld)
                    nodeLock.ExitUpgradeableReadLock();
                else
                    nodeLock.ExitReadLock();
            }
        }
        finally
        {
            _structureLock.ExitWriteLock();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Attempts to get the value associated with the specified key
    /// </summary>
    /// <param name="key">The key of the item to get (case-sensitive)</param>
    /// <param name="value">The value associated with <paramref name="key"/>, if found</param>
    /// <returns>
    /// True if the key was found, otherwise false
    /// </returns>
    public virtual bool TryGetValue(string key, out TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        value = default;

        return Find(key, _root, out var node) && node.TryGetValue(out value);
    }

    /// <summary>
    /// Adds a key-value pair to the trie
    /// </summary>
    /// <param name="key">The key of the new item (case-sensitive)</param>
    /// <param name="value">The value to be associated with <paramref name="key"/></param>
    public virtual void Add(string key, TValue value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));

        GetOrAddNode(key, value, true);
    }

    /// <summary>
    /// Clears the trie
    /// </summary>
    public virtual void Clear()
    {
        _root = new TrieNode();
    }

    /// <summary>
    /// Gets all key-value pairs for keys starting with the given prefix
    /// </summary>
    /// <param name="prefix">The prefix (case-sensitive) to search for</param>
    /// <returns>
    /// All key-value pairs for keys starting with <paramref name="prefix"/>
    /// </returns>
    public virtual IEnumerable<KeyValuePair<string, TValue>> Search(string prefix)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        if (!SearchOrPrune(prefix, false, out var node))
            return Enumerable.Empty<KeyValuePair<string, TValue>>();

        // depth-first traversal
        IEnumerable<KeyValuePair<string, TValue>> traverse(TrieNode n, string s)
        {
            if (n.TryGetValue(out var value))
                yield return new KeyValuePair<string, TValue>(s, value);

            var nLock = GetLock(n);
            nLock.EnterReadLock();
            List<TrieNode> children;

            try
            {
                // we can't know what is done during enumeration, so we need to make a copy of the children
                children = n.Children.Values.ToList();
            }
            finally
            {
                nLock.ExitReadLock();
            }

            foreach (var child in children)
            foreach (var kv in traverse(child, s + child.Label))
                yield return kv;
        }

        return traverse(node, node.Label);
    }

    /// <summary>
    /// Removes the item with the given key, if present
    /// </summary>
    /// <param name="key">The key (case-sensitive) of the item to be removed</param>
    public void Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));

        Remove(_root, key);
    }

    /// <summary>
    /// Attempts to remove all items with keys starting with the specified prefix
    /// </summary>
    /// <param name="prefix">The prefix (case-sensitive) of the items to be deleted</param>
    /// <param name="subCollection">The sub-collection containing all deleted items, if found</param>
    /// <returns>
    /// True if the prefix was successfully removed from the trie, otherwise false
    /// </returns>
    public virtual bool Prune(string prefix, out IConcurrentCollection<TValue> subCollection)
    {
        var succeeded = SearchOrPrune(prefix, true, out var subtreeRoot);
        subCollection = succeeded ? new ConcurrentTrie<TValue>(subtreeRoot) : default;
        return succeeded;
    }

    protected bool SearchOrPrune(string prefix, bool prune, out TrieNode subtreeRoot)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        if (prefix.Length == 0)
        {
            subtreeRoot = _root;
            return true;
        }

        subtreeRoot = default;
        var node = _root;
        var parent = node;
        var span = prefix.AsSpan();
        var i = 0;

        while (i < span.Length)
        {
            var c = span[i];
            var parentLock = GetLock(parent);
            parentLock.EnterUpgradeableReadLock();

            try
            {
                if (!parent.Children.TryGetValue(c, out node))
                    return false;

                var label = node.Label.AsSpan();
                var k = GetCommonPrefixLength(span[i..], label);

                if (k == span.Length - i)
                {
                    subtreeRoot = new TrieNode(prefix[..i] + node.Label, node);
                    if (!prune)
                        return true;

                    parentLock.EnterWriteLock();

                    try
                    {
                        if (parent.Children.Remove(c, out node))
                            return true;
                    }
                    finally
                    {
                        parentLock.ExitWriteLock();
                    }

                    // was removed by another thread
                    return false;
                }

                if (k < label.Length)
                    return false;

                i += label.Length;
            }
            finally
            {
                parentLock.ExitUpgradeableReadLock();
            }

            parent = node;
        }

        return false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a collection that contains the keys in the <see cref="ConcurrentTrie{TValue}" />
    /// </summary>
    public IEnumerable<string> Keys => Search(string.Empty).Select(t => t.Key);

    #endregion

    #region Nested classes

    /// <summary>
    /// A striped ReaderWriterLock wrapper
    /// </summary>
    protected class StripedReaderWriterLock
    {
        #region Fields

        protected const int MULTIPLIER = 8;
        protected readonly ReaderWriterLockSlim[] _locks;

        #endregion

        #region Ctor

        // defaults to 8 times the number of processor cores
        public StripedReaderWriterLock(int nLocks = 0)
        {
            if (nLocks == 0)
                nLocks = Environment.ProcessorCount * MULTIPLIER;

            _locks = new ReaderWriterLockSlim[nLocks];

            for (var i = 0; i < nLocks; i++)
                _locks[i] = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a lock on the object
        /// </summary>
        public ReaderWriterLockSlim GetLock(object obj)
        {
            return _locks[obj.GetHashCode() % _locks.Length];
        }

        #endregion
    }

    /// <summary>
    /// An implementation of a trie node
    /// </summary>
    protected class TrieNode
    {
        #region Fields

        // used to avoid keeping a separate boolean flag that would use another byte per node
        protected static readonly ValueWrapper _deleted = new(default);
        protected volatile ValueWrapper _value;

        #endregion

        #region Ctor

        public TrieNode(string label = "")
        {
            Label = label;
            Children = new Dictionary<char, TrieNode>();
        }

        public TrieNode(ReadOnlySpan<char> label) : this(label.ToString())
        {
        }

        public TrieNode(string label, TrieNode node) : this(label)
        {
            Children = node.Children;
            _value = node._value;
        }

        public TrieNode(ReadOnlySpan<char> label, TrieNode node) : this(label)
        {
            Children = node.Children;
            _value = node._value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to get the node value
        /// </summary>
        /// <param name="value">The node value, if value exists</param>
        /// <returns>
        /// True if value exists, otherwise false
        /// </returns>
        public bool TryGetValue(out TValue value)
        {
            var wrapper = _value;
            value = default;

            if (wrapper == null)
                return false;

            value = wrapper.Value;

            return true;
        }

        public bool TryRemoveValue(out TValue value)
        {
            var wrapper = Interlocked.Exchange(ref _value, null);
            value = default;

            if (wrapper == null)
                return false;

            value = wrapper.Value;

            return true;
        }

        public void SetValue(TValue value)
        {
            _value = new ValueWrapper(value);
        }

        public TValue GetOrAddValue(TValue value)
        {
            var wrapper = Interlocked.CompareExchange(ref _value, new ValueWrapper(value), null);

            return wrapper != null ? wrapper.Value : value;
        }

        public void Delete()
        {
            _value = _deleted;
        }

        #endregion

        #region Properties

        public Dictionary<char, TrieNode> Children { get; }

        public string Label { get; }

        public bool IsDeleted => _value == _deleted;

        public bool HasValue => _value != null && !IsDeleted;

        #endregion

        #region Nested class

        protected class ValueWrapper
        {
            public readonly TValue Value;

            public ValueWrapper(TValue value)
            {
                Value = value;
            }
        }

        #endregion
    }

    #endregion
}