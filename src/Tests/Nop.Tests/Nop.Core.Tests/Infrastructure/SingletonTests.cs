using FluentAssertions;
using Nop.Core.Infrastructure;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Infrastructure;

[TestFixture]
public class SingletonTests
{
    [Test]
    public void SingletonIsNullByDefault()
    {
        var instance = Singleton<SingletonTests>.Instance;
        instance.Should().BeNull();
    }

    [Test]
    public void SingletonsShareSameSingletonsDictionary()
    {
        Singleton<int>.Instance = 1;
        Singleton<double>.Instance = 2.0;

        Singleton<int>.AllSingletons.Should().BeSameAs(Singleton<double>.AllSingletons);
        BaseSingleton.AllSingletons[typeof(int)].Should().Be(1);
        BaseSingleton.AllSingletons[typeof(double)].Should().Be(2.0M);
    }

    [Test]
    public void SingletonDictionaryIsCreatedByDefault()
    {
        var instance = SingletonDictionary<SingletonTests, object>.Instance;
        instance.Should().NotBeNull();
    }

    [Test]
    public void SingletonDictionaryCanStoreStuff()
    {
        var instance = SingletonDictionary<Type, SingletonTests>.Instance;
        instance[typeof(SingletonTests)] = this;
        instance[typeof(SingletonTests)].Should().BeSameAs(this);
    }

    [Test]
    public void SingletonListIsCreatedByDefault()
    {
        var instance = SingletonList<SingletonTests>.Instance;
        instance.Should().NotBeNull();
    }

    [Test]
    public void SingletonListCanStoreItems()
    {
        var instance = SingletonList<SingletonTests>.Instance;
        instance.Insert(0, this);
        instance[0].Should().BeSameAs(this);
    }
}