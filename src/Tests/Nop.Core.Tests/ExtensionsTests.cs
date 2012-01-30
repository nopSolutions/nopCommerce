using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void Can_check_IsNullOrDefault()
        {
            int? x1 = null;
            x1.IsNullOrDefault().ShouldBeTrue();

            int? x2 = 0;
            x2.IsNullOrDefault().ShouldBeTrue();

            int? x3 = 1;
            x3.IsNullOrDefault().ShouldBeFalse();
        }
    }
}



