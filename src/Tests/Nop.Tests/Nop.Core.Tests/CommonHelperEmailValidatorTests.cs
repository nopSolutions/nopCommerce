using FluentAssertions;
using Nop.Core;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests
{
    [TestFixture]
    public class CommonHelperEmailValidatorTests
    {
        [Test]
        public void WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass()
        {
            var email = "testperson@gmail.com";
            var result = CommonHelper.IsValidEmail(email);
            result.Should().BeTrue();
        }

        [Test]
        public void WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass()
        {
            var email = "testperson+label@gmail.com";
            var result = CommonHelper.IsValidEmail(email);
            result.Should().BeTrue();
        }

        [Test]
        public void WhenTheTextIsNullThenTheValidatorShouldFail()
        {
            var result = CommonHelper.IsValidEmail(null);
            result.Should().BeFalse();
        }

        [Test]
        public void WhenTheTextIsEmptyThenTheValidatorShouldFail()
        {
            var email = string.Empty;
            var result = CommonHelper.IsValidEmail(email);
            result.Should().BeFalse();
        }

        [Test]
        public void WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail()
        {
            const string email = "testperso";
            var result = CommonHelper.IsValidEmail(email);
            result.Should().BeFalse();
        }

        [Test]
        public void ThisShouldNotHang()
        {
            const string email = "thisisaverylongstringcodeplex.com";
            var result = CommonHelper.IsValidEmail(email);
            result.Should().BeFalse();
        }

        [Test]
        public void WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass()
        {
            var email = "testperson@gmail.com";
            var result = CommonHelper.IsValidEmail(email);
            result.Should().BeTrue();

            email = "TestPerson@gmail.com";
            result = CommonHelper.IsValidEmail(email);
            result.Should().BeTrue();
        }
    }
}
