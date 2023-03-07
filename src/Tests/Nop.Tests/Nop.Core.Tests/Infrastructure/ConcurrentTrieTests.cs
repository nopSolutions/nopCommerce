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
        }

        [Test]
        public void CanRemoveValue()
        {
            var sut = new ConcurrentTrie<int>();
            sut.Add("a", 1);
            sut.Add("ab", 1);
            sut.Add("abc", 1);
            sut.Remove("ab");
            sut.TryGetValue("ab", out _).Should().BeFalse();
            sut.TryGetValue("abc", out _).Should().BeTrue();
            sut.TryGetValue("a", out _).Should().BeTrue();
            Assert.DoesNotThrow(() => sut.Remove("ab"));
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
            sut.Add("ab", 1);
            sut.Add("abc", 1);
            sut.Add("abd", 1);
            sut.Prune("ab", out var subtree).Should().BeTrue();
            subtree.Keys.Should().BeEquivalentTo(new[] { "ab", "abc", "abd" });
            sut.Keys.Should().BeEquivalentTo(new[] { "a", "b" });
            sut.Prune("b", out subtree).Should().BeTrue();
            subtree.Keys.Should().BeEquivalentTo(new[] { "b" });
            sut.Keys.Should().BeEquivalentTo(new[] { "a" });
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
    }
}
