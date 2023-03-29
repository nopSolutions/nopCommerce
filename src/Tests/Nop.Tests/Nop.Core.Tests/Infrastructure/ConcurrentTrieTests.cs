using System.Diagnostics;
using FluentAssertions;
using Nop.Core.Infrastructure;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Infrastructure
{
    [TestFixture]
    public class ConcurrentTrieTests
    {
        [Test]
        public void CanAddAndGetValue()
        {
            var sut = new ConcurrentTrie<int>();
            sut.TryGetValue("a", out _).Should().BeFalse();
            sut.Add("a", 1);
            sut.TryGetValue("a", out var value).Should().BeTrue();
            value.Should().Be(1);
            sut.Add("a", 2);
            sut.TryGetValue("a", out value).Should().BeTrue();
            value.Should().Be(2);
        }

        [Test]
        public void CanAddAndGetValues()
        {
            var sut = new ConcurrentTrie<int>();
            sut.Add("a", 1);
            sut.TryGetValue("ab", out _).Should().BeFalse();
            sut.Add("abc", 3);
            sut.TryGetValue("ab", out _).Should().BeFalse();
            sut.TryGetValue("a", out var value).Should().BeTrue();
            value.Should().Be(1);
            sut.TryGetValue("abc", out value).Should().BeTrue();
            value.Should().Be(3);
            sut.Add("ab", 2);
            sut.TryGetValue("ab", out value).Should().BeTrue();
            value.Should().Be(2);
        }

        [Test]
        public void DoesNotBlockWhileEnumerating()
        {
            var sut = new ConcurrentTrie<int>();
            sut.Add("a", 0);
            sut.Add("ab", 0);
            foreach (var item in sut.Keys)
                sut.Remove(item);
        }

        [Test]
        public void CanRemoveValue()
        {
            var sut = new ConcurrentTrie<int>();
            sut.Add("a", 1);
            sut.Add("b", 1);
            sut.Add("bbb", 1);
            sut.Add("ab", 1);
            sut.Add("aa", 1);
            sut.Add("abc", 1);
            sut.Add("abb", 1);
            sut.Remove("ab");
            sut.TryGetValue("ab", out _).Should().BeFalse();
            sut.Keys.Should().BeEquivalentTo(new[] { "abc", "a", "b", "aa", "abb", "bbb" });
            Assert.DoesNotThrow(() => sut.Remove("ab"));
            sut.Remove("bb");
            sut.TryGetValue("b", out _).Should().BeTrue();
            sut.TryGetValue("bbb", out _).Should().BeTrue();

            sut.Prune("b", out sut);
            sut.Keys.Should().BeEquivalentTo(new[] { "b", "bbb" });
            sut.Remove("b");
            sut.Keys.Should().BeEquivalentTo(new[] { "bbb" });
        }

        [Test]
        public void CanGetKeys()
        {
            var sut = new ConcurrentTrie<int>();
            var keys = new[] { "a", "b", "abc" };
            foreach (var key in keys)
                sut.Add(key, 1);
            sut.Keys.Should().BeEquivalentTo(keys);
        }

        [Test]
        public void CanGetValues()
        {
            var sut = new ConcurrentTrie<int>();
            var keys = new[] { "a", "b", "abc" };
            var values = new[] { 1, 2, 3 };
            foreach (var (key, value) in keys.Zip(values))
                sut.Add(key, value);
            sut.Values.Should().BeEquivalentTo(values);
        }

        [Test]
        public void CanPrune()
        {
            var sut = new ConcurrentTrie<int>();
            sut.Add("a", 1);
            sut.Add("b", 1);
            sut.Add("bba", 1);
            sut.Add("bbb", 1);
            sut.Add("ab", 1);
            sut.Add("abc", 1);
            sut.Add("abd", 1);
            sut.Prune("bc", out _).Should().BeFalse();
            sut.Prune("ab", out var subtree).Should().BeTrue();
            subtree.Keys.Should().BeEquivalentTo(new[] { "ab", "abc", "abd" });
            sut.Keys.Should().BeEquivalentTo(new[] { "a", "b", "bba", "bbb" });
            sut.Prune("b", out subtree).Should().BeTrue();
            subtree.Keys.Should().BeEquivalentTo(new[] { "b", "bba", "bbb" });
            sut.Keys.Should().BeEquivalentTo(new[] { "a" });

            sut = subtree;
            sut.Prune("bb", out subtree).Should().BeTrue();
            subtree.Keys.Should().BeEquivalentTo(new[] { "bba", "bbb" });
            sut.Keys.Should().BeEquivalentTo(new[] { "b" });
            sut = subtree;
            sut.Prune("bba", out subtree);
            subtree.Keys.Should().BeEquivalentTo(new[] { "bba" });
            sut.Keys.Should().BeEquivalentTo(new[] { "bbb" });

            sut = new ConcurrentTrie<int>();
            sut.Add("aaa", 1);
            sut.Prune("a", out subtree).Should().BeTrue();
            subtree.Keys.Should().BeEquivalentTo(new[] { "aaa" });
            sut.Keys.Should().BeEmpty();
            sut = subtree;
            sut.Prune("aa", out subtree).Should().BeTrue();
            sut.Keys.Should().BeEmpty();
            subtree.Keys.Should().BeEquivalentTo(new[] { "aaa" });
        }

        [Test]
        public void CanSearch()
        {
            var sut = new ConcurrentTrie<int>();
            sut.Add("a", 1);
            sut.Add("b", 1);
            sut.Add("ab", 1);
            sut.Add("abc", 2);
            var keys = sut.Keys.ToList();
            sut.Search("ab").Should().BeEquivalentTo(new KeyValuePair<string, int>[]
            {
                new("ab", 1),
                new("abc", 2)
            });
            sut.Keys.Should().BeEquivalentTo(keys);
        }

        [Test]
        public void CanClear()
        {
            var sut = new ConcurrentTrie<int>();
            sut.Add("a", 1);
            sut.Add("ab", 1);
            sut.Add("abc", 1);
            sut.Clear();
            sut.Keys.Should().BeEmpty();
        }

        [Test]
        [Ignore("Not a test, used for profiling.")]
        public void Profile()
        {
            var sut = new ConcurrentTrie<byte>();
            var sw = new Stopwatch();
            var memory = GC.GetTotalMemory(true);
            sw.Start();
            for (var i = 0; i < 1000000; i++)
                sut.Add(Guid.NewGuid().ToString(), 0);
            sw.Stop();
            var delta = GC.GetTotalMemory(true) - memory;
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.WriteLine(delta);
        }

        [Test]
        [Ignore("Concurrency tests are inherently flaky, and may give false positives. Enable manually when needed.")]
        public void DoesNotBreakDuringParallelAddRemove()
        {
            var sut = new ConcurrentTrie<int>();
            var sw = new Stopwatch();
            var memory = GC.GetTotalMemory(true);
            sw.Start();
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
            sw.Stop();
            var delta = GC.GetTotalMemory(true) - memory;
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.WriteLine(delta);
            sut.Keys.Count().Should().Be(0);
        }

        [Test]
        [Ignore("Concurrency tests are inherently flaky, and may give false positives. Enable manually when needed.")]
        public void DoesNotBreakDuringParallelAddPrune()
        {
            var sut = new ConcurrentTrie<int>();
            var sw = new Stopwatch();
            var memory = GC.GetTotalMemory(true);
            sw.Start();
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
            sw.Stop();
            var delta = GC.GetTotalMemory(true) - memory;
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.WriteLine(delta);
            sut.Keys.Count().Should().Be(0);
        }
    }
}
