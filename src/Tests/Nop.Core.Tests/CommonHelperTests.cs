using FluentAssertions;
using NUnit.Framework;

namespace Nop.Core.Tests
{
    [TestFixture]
    public class CommonHelperTests
    {
        [Test]
        public void Can_get_typed_value()
        {
            CommonHelper.To<int>("1000").Should().BeOfType(typeof(int));
            CommonHelper.To<int>("1000").Should().Be(1000);
        }
    }
}
