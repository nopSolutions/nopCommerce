using System.Collections.Concurrent;
using System.Diagnostics;
using FluentAssertions;
using Nop.Core.Infrastructure;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Infrastructure
{
    [TestFixture]
    public class ConcurrentTrieTests
    {
        private IConcurrentCollection<int> _sut;

        private void Profile(Action action)
        {
            var sw = new Stopwatch();
            var memory = GC.GetTotalMemory(true) / 1024.0 / 1024.0;
            sw.Start();
            
            action.Invoke();

            sw.Stop();
            var delta = GC.GetTotalMemory(true) / 1024.0 / 1024.0 - memory;
            Console.WriteLine("Elapsed teme: {0:F}s", sw.ElapsedMilliseconds / 1000.0);
            Console.WriteLine("Memory usege: {0:F}Mb", delta);
        }

        [SetUp]
        public void SetUp()
        {
            _sut = new ConcurrentTrie<int>();
        }

        [Test]
        public void CanAddAndGetValue()
        {
            _sut.TryGetValue("a", out _).Should().BeFalse();
            _sut.Add("a", 1);
            _sut.TryGetValue("a", out var value).Should().BeTrue();
            value.Should().Be(1);
            _sut.Add("a", 2);
            _sut.TryGetValue("a", out value).Should().BeTrue();
            value.Should().Be(2);
        }

        [Test]
        public void CanAddAndGetValues()
        {
            _sut.Add("a", 1);
            _sut.TryGetValue("ab", out _).Should().BeFalse();
            _sut.Add("abc", 3);
            _sut.TryGetValue("ab", out _).Should().BeFalse();
            _sut.TryGetValue("a", out var value).Should().BeTrue();
            value.Should().Be(1);
            _sut.TryGetValue("abc", out value).Should().BeTrue();
            value.Should().Be(3);
            _sut.Add("ab", 2);
            _sut.TryGetValue("ab", out value).Should().BeTrue();
            value.Should().Be(2);
        }

        [Test]
        public void DoesNotBlockWhileEnumerating()
        {
            _sut.Add("a", 0);
            _sut.Add("ab", 0);
            foreach (var item in _sut.Keys)
                _sut.Remove(item);
        }

        [Test]
        public void CanRemoveValue()
        {
            _sut.Add("a", 1);
            _sut.Add("b", 1);
            _sut.Add("bbb", 1);
            _sut.Add("ab", 1);
            _sut.Add("aa", 1);
            _sut.Add("abc", 1);
            _sut.Add("abb", 1);
            _sut.Remove("ab");
            _sut.TryGetValue("ab", out _).Should().BeFalse();
            _sut.Keys.Should().BeEquivalentTo(new[] { "abc", "a", "b", "aa", "abb", "bbb" });
            Assert.DoesNotThrow(() => _sut.Remove("ab"));
            _sut.Remove("bb");
            _sut.TryGetValue("b", out _).Should().BeTrue();
            _sut.TryGetValue("bbb", out _).Should().BeTrue();

            _sut.Prune("b", out _sut);
            _sut.Keys.Should().BeEquivalentTo(new[] { "b", "bbb" });
            _sut.Remove("b");
            _sut.Keys.Should().BeEquivalentTo(new[] { "bbb" });
        }

        [Test]
        public void CanGetKeys()
        {
            var keys = new[] { "a", "b", "abc" };
            foreach (var key in keys)
                _sut.Add(key, 1);
            _sut.Keys.Should().BeEquivalentTo(keys);
        }
        
        [Test]
        public void CanPrune()
        {
            _sut.Add("a", 1);
            _sut.Add("b", 1);
            _sut.Add("bba", 1);
            _sut.Add("bbb", 1);
            _sut.Add("ab", 1);
            _sut.Add("abc", 1);
            _sut.Add("abd", 1);
            _sut.Prune("bc", out _).Should().BeFalse();
            _sut.Prune("ab", out var subtree).Should().BeTrue();
            subtree.Keys.Should().BeEquivalentTo(new[] { "ab", "abc", "abd" });
            _sut.Keys.Should().BeEquivalentTo(new[] { "a", "b", "bba", "bbb" });
            _sut.Prune("b", out subtree).Should().BeTrue();
            subtree.Keys.Should().BeEquivalentTo(new[] { "b", "bba", "bbb" });
            _sut.Keys.Should().BeEquivalentTo(new[] { "a" });

            _sut = subtree;
            _sut.Prune("bb", out subtree).Should().BeTrue();
            subtree.Keys.Should().BeEquivalentTo(new[] { "bba", "bbb" });
            _sut.Keys.Should().BeEquivalentTo(new[] { "b" });
            _sut = subtree;
            _sut.Prune("bba", out subtree);
            subtree.Keys.Should().BeEquivalentTo(new[] { "bba" });
            _sut.Keys.Should().BeEquivalentTo(new[] { "bbb" });

            _sut = new ConcurrentTrie<int>();
            _sut.Add("aaa", 1);
            _sut.Prune("a", out subtree).Should().BeTrue();
            subtree.Keys.Should().BeEquivalentTo(new[] { "aaa" });
            _sut.Keys.Should().BeEmpty();
            _sut = subtree;
            _sut.Prune("aa", out subtree).Should().BeTrue();
            _sut.Keys.Should().BeEmpty();
            subtree.Keys.Should().BeEquivalentTo(new[] { "aaa" });
        }

        [Test]
        public void CanSearch()
        {
            _sut.Add("a", 1);
            _sut.Add("b", 1);
            _sut.Add("ab", 1);
            _sut.Add("abc", 2);
            var keys = _sut.Keys.ToList();
            _sut.Search("ab").Should().BeEquivalentTo(new KeyValuePair<string, int>[]
            {
                new("ab", 1),
                new("abc", 2)
            });
            _sut.Keys.Should().BeEquivalentTo(keys);
        }

        [Test]
        public void CanClear()
        {
            _sut.Add("a", 1);
            _sut.Add("ab", 1);
            _sut.Add("abc", 1);
            _sut.Clear();
            _sut.Keys.Should().BeEmpty();
        }

        [Test]
        [TestCase(typeof(NopConcurrentCollection<byte>))]
        [TestCase(typeof(ConcurrentTrie<byte>))]
        [Ignore("Not a test, used for profiling.")]
        public void Profile(Type oType)
        {
            var sut = Activator.CreateInstance(oType) as IConcurrentCollection<byte>;
            sut.Should().NotBeNull();

            Profile(() =>
            {
                for (var i = 0; i < 1000000; i++)
                    sut.Add(Guid.NewGuid().ToString(), 0);
            });
        }

        [Test]
        [TestCase(typeof(NopConcurrentCollection<int>))]
        [TestCase(typeof(ConcurrentTrie<int>))]
        [Ignore("Not a test, used for profiling.")]
        public void DoesNotBreakDuringParallelAddRemove(Type oType)
        {
            var sut = Activator.CreateInstance(oType) as IConcurrentCollection<int>;
            sut.Should().NotBeNull();

            Profile(() =>
            {
                Parallel.For(0, 1000, new ParallelOptions { MaxDegreeOfParallelism = 8 }, j =>
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        var s = $"{i}-{j}";
                        sut.Add(s, i);
                        sut.TryGetValue(s, out var value).Should().BeTrue();
                        value.Should().Be(i);
                        sut.Remove(s);
                        sut.TryGetValue(s, out _).Should().BeFalse();
                    }
                });
            });
            
            sut.Keys.Count().Should().Be(0);
        }

        [Test]
        [TestCase(typeof(NopConcurrentCollection<int>))]
        [TestCase(typeof(ConcurrentTrie<int>))]
        [Ignore("Not a test, used for profiling.")]
        public void DoesNotBreakDuringParallelAddPrune(Type oType)
        {
            var sut = Activator.CreateInstance(oType) as IConcurrentCollection<int>;
            sut.Should().NotBeNull();

            Profile(() =>
            {
                Parallel.For(0, 1000, new ParallelOptions { MaxDegreeOfParallelism = 8 }, j =>
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        var s = $"{j}-{i}";
                        sut.Add(s, i);
                    }
                    sut.Prune($"{j}-", out var st);
                    st.Keys.Count().Should().Be(1000);
                });
            });
            
            sut.Keys.Count().Should().Be(0);
        }

        [Test]
        [TestCase(typeof(NopConcurrentCollection<byte>))]
        [TestCase(typeof(ConcurrentTrie<byte>))]
        [Ignore("Not a test, used for profiling.")]
        public void ProfilePrune(Type oType)
        {
            var sut = Activator.CreateInstance(oType) as IConcurrentCollection<byte>;
            sut.Should().NotBeNull();
            // insert
            for (var i = 0; i < 10000; i++)
                sut.Add(Guid.NewGuid().ToString(), 0);

            Profile(() =>
            {
                Parallel.For(0, 1000, new ParallelOptions { MaxDegreeOfParallelism = 8 }, j =>
                {
                    for (var i = 0; i < 20; i++)
                    {
                        // insert
                        sut.Add(Guid.NewGuid().ToString(), 0);

                        // remove by prefix
                        sut.Prune(Guid.NewGuid().ToString()[..5], out _);
                    }
                });
            });
        }

        #region Nested class

        /// <summary>
        /// A thread-safe collection based on <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// </summary>
        public partial class NopConcurrentCollection<TValue> : IConcurrentCollection<TValue>
        {
            #region Fields

            protected readonly ConcurrentDictionary<string, TValue> _dictionary;

            #endregion

            #region Ctor

            /// <summary>
            /// Initializes a new instance of <see cref="NopConcurrentCollection{TValue}" />
            /// </summary>
            protected NopConcurrentCollection(IEnumerable<KeyValuePair<string, TValue>> subCollection)
            {
                _dictionary = new ConcurrentDictionary<string, TValue>(subCollection);
            }

            /// <summary>
            /// Initializes a new empty instance of <see cref="NopConcurrentCollection{TValue}" />
            /// </summary>
            public NopConcurrentCollection()
            {
                _dictionary = new ConcurrentDictionary<string, TValue>();
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
            public bool TryGetValue(string key, out TValue value)
            {
                return _dictionary.TryGetValue(key, out value);
            }

            /// <summary>
            /// Adds a key-value pair to the collection
            /// </summary>
            /// <param name="key">The key of the new item (case-sensitive)</param>
            /// <param name="value">The value to be associated with <paramref name="key"/></param>
            public void Add(string key, TValue value)
            {
                _dictionary[key] = value;
            }

            /// <summary>
            /// Clears the collection
            /// </summary>
            public void Clear()
            {
                _dictionary.Clear();
            }

            /// <summary>
            /// Gets all key-value pairs for keys starting with the given prefix
            /// </summary>
            /// <param name="prefix">The prefix (case-sensitive) to search for</param>
            /// <returns>
            /// All key-value pairs for keys starting with <paramref name="prefix"/>
            /// </returns>
            public IEnumerable<KeyValuePair<string, TValue>> Search(string prefix)
            {
                return _dictionary.Keys.Where(k => k.StartsWith(prefix))
                    .Select(key => new KeyValuePair<string, TValue>(key, _dictionary[key]));
            }

            /// <summary>
            /// Removes the item with the given key, if present
            /// </summary>
            /// <param name="key">The key (case-sensitive) of the item to be removed</param>
            public void Remove(string key)
            {
                _dictionary.Remove(key, out _);
            }

            /// <summary>
            /// Attempts to remove all items with keys starting with the specified prefix
            /// </summary>
            /// <param name="prefix">The prefix (case-sensitive) of the items to be deleted</param>
            /// <param name="subCollection">The sub-collection containing all deleted items, if found</param>
            /// <returns>
            /// True if the prefix was successfully removed from the collection, otherwise false
            /// </returns>
            public bool Prune(string prefix, out IConcurrentCollection<TValue> subCollection)
            {
                subCollection = new NopConcurrentCollection<TValue>(Search(prefix));

                try
                {
                    foreach (var key in subCollection.Keys)
                        Remove(key);
                }
                catch
                {
                    return false;
                }

                return subCollection.Keys.Any();
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets a collection that contains the keys in the <see cref="IConcurrentCollection{TValue}" />
            /// </summary>
            public IEnumerable<string> Keys => _dictionary.Keys;

            #endregion
        }

        #endregion
    }
}
