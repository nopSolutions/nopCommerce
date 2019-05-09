using System;
using NUnit.Framework;

namespace Nop.Tests
{
    public static class TypeAssert
    {
        public static void AreEqual(Type expected, object instance)
        {
            if (expected == null)
                Assert.IsNull(instance);
            else
                Assert.IsNotNull(instance, "Instance was null");
            Assert.AreEqual(expected, instance.GetType(), "Expected: " + expected + ", was: " + instance.GetType() + " was not of type " + instance.GetType());
        }
    }
}
