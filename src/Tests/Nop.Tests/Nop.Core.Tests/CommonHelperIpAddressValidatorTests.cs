using FluentAssertions;
using Nop.Core;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests
{
    [TestFixture]
    public class CommonHelperIpAddressValidatorTests
    {
        [Test]
        public void WhenTheTextIsAValidIpv4AddressThenTheValidatorShouldPass()
        {
            const string ip = "123.123.123.123";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.Should().BeTrue();
        }

        [Test]
        public void WhenTheTextIsAValidIpv6AddressThenTheValidatorShouldPass()
        {
            const string ip = "FE80:0000:0000:0000:0202:B3FF:FE1E:8329";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.Should().BeTrue();
        }

        [Test]
        public void WhenTheTextIsNotAValidIpAddressThenTheValidatorShouldFail()
        {
            const string ip = "abc";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.Should().BeFalse();
        }

        [Test]
        public void WhenTheTextIsAnIpAddressButWithWrongRangeThenTheValidatorShouldFail()
        {
            const string ip = "999.999.999.999";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.Should().BeFalse();
        }
    }
}