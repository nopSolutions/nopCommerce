using System;
using NUnit.Framework;

namespace Nop.Tests
{
    public static class TypeAssert
    {
        public static void AreEqual(object expected, object instance)
        {
            if (expected == null)
                Assert.IsNull(instance);
            else
                Assert.IsNotNull(instance, "Instance was null");
            Assert.AreEqual(expected.GetType(), instance.GetType(), "Expected: " + expected.GetType() + ", was: " + instance.GetType() + " was not of type " + instance.GetType());
        }

        public static void AreEqual(Type expected, object instance)
        {
            if (expected == null)
                Assert.IsNull(instance);
            else
                Assert.IsNotNull(instance, "Instance was null");
            Assert.AreEqual(expected, instance.GetType(), "Expected: " + expected + ", was: " + instance.GetType() + " was not of type " + instance.GetType());
        }

        public static void Equals<T>(object instance)
        {
            AreEqual(typeof(T), instance);
        }

        public static void Is<T>(object instance)
        {
            Assert.IsTrue(instance is T, "Instance " + instance + " was not of type " + typeof(T));
        }
    }
}
