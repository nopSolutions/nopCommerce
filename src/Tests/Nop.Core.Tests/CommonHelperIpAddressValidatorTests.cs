using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests
{
    [TestFixture]
    public class CommonHelperIpAddressValidatorTests
    {
        [Test]
        public void When_the_text_is_a_valid_ipv4_address_then_the_validator_should_pass()
        {
            string ip = "123.123.123.123";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.ShouldEqual(true);
        }

        [Test]
        public void When_the_text_is_a_valid_ipv6_address_then_the_validator_should_pass()
        {
            string ip = "FE80:0000:0000:0000:0202:B3FF:FE1E:8329";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.ShouldEqual(true);
        }

        [Test]
        public void When_the_text_is_not_a_valid_ip_address_then_the_validator_should_fail()
        {
            string ip = "abc";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.ShouldEqual(false);
        }

        [Test]
        public void When_the_text_is_an_ip_address_but_with_wrong_range_then_the_validator_should_fail()
        {
            string ip = "999.999.999.999";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.ShouldEqual(false);
        }
    }
}