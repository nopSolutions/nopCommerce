using System;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests
{
    [TestFixture]
    public class CommonHelperEmailValidatorTests
    {
        [Test]
        public void When_the_text_is_a_valid_email_address_then_the_validator_should_pass()
        {
            string email = "testperson@gmail.com";
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(true);
        }

        [Test]
        public void When_the_text_is_a_valid_email_address_including_plus_validator_should_pass()
        {
            string email = "testperson+label@gmail.com";
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(true);
        }

        [Test]
        public void When_the_text_is_null_then_the_validator_should_fail()
        {
            string email = null;
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(false);
        }

        [Test]
        public void When_the_text_is_empty_then_the_validator_should_fail()
        {
            string email = String.Empty;
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(false);
        }

        [Test]
        public void When_the_text_is_not_a_valid_email_address_then_the_validator_should_fail()
        {
            string email = "testperso";
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(false);
        }


        [Test]
        public void This_should_not_hang()
        {
            string email = "thisisaverylongstringcodeplex.com";
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(false);
        }

        [Test]
        public void When_email_address_contains_upper_cases_then_the_validator_should_pass()
        {
            string email = "testperson@gmail.com";
            var result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(true);

            email = "TestPerson@gmail.com";
            result = CommonHelper.IsValidEmail(email);
            result.ShouldEqual(true);
        }
    }
}
