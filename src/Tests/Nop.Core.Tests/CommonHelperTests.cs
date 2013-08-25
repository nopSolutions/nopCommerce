using NUnit.Framework;
using Nop.Tests;

namespace Nop.Core.Tests
{
    [TestFixture]
    public class CommonHelperTests
    {
        [Test]
        public void Can_get_typed_value()
        {
            CommonHelper.To<int>("1000").ShouldBe<int>();
            CommonHelper.To<int>("1000").ShouldEqual(1000);
        }
    }
}
