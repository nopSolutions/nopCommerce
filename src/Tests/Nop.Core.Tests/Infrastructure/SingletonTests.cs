using System;
using Nop.Core.Infrastructure;
using NUnit.Framework;

namespace Nop.Core.Tests.Infrastructure
{
    [TestFixture]
    public class SingletonTests
    {
        [Test]
        public void Singleton_IsNullByDefault()
        {
            var instance = Singleton<SingletonTests>.Instance;
            Assert.That(instance, Is.Null);
        }

        [Test]
        public void Singletons_ShareSame_SingletonsDictionary()
        {
            Singleton<int>.Instance = 1;
            Singleton<double>.Instance = 2.0;

            Assert.That(Singleton<int>.AllSingletons, Is.SameAs(Singleton<double>.AllSingletons));
            Assert.That(Singleton.AllSingletons[typeof(int)], Is.EqualTo(1));
            Assert.That(Singleton.AllSingletons[typeof(double)], Is.EqualTo(2.0));
        }

        [Test]
        public void SingletonDictionary_IsCreatedByDefault()
        {
            var instance = SingletonDictionary<SingletonTests, object>.Instance;
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void SingletonDictionary_CanStoreStuff()
        {
            var instance = SingletonDictionary<Type, SingletonTests>.Instance;
            instance[typeof(SingletonTests)] = this;
            Assert.That(instance[typeof(SingletonTests)], Is.SameAs(this));
        }

        [Test]
        public void SingletonList_IsCreatedByDefault()
        {
            var instance = SingletonList<SingletonTests>.Instance;
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void SingletonList_CanStoreItems()
        {
            var instance = SingletonList<SingletonTests>.Instance;
            instance.Insert(0, this);
            Assert.That(instance[0], Is.SameAs(this));
        }
    }
}
