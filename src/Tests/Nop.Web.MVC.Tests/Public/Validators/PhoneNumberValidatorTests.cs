using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Validators;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators
{
    [TestFixture]
    public class PhoneNumberValidatorTests
    {
        private TestValidator _validator;
        private CustomerSettings _customerSettings;

        [SetUp]
        public void Setup()
        {
            _customerSettings = new CustomerSettings
            {
                PhoneNumberValidationRule = "^[0-9]{1,14}?$",
                PhoneNumberValidationEnabled = true,
                PhoneNumberValidationUseRegex = false
            };

            _validator = new TestValidator { v => v.RuleFor(x => x.PhoneNumber).IsPhoneNumber(_customerSettings) };
        }

        [Test]
        public void IsValidTests()
        {
            //optional value is not valid
            _validator.Validate(new Person { PhoneNumber = null }).IsValid.Should().BeFalse();
            _validator.Validate(new Person { PhoneNumber = string.Empty }).IsValid.Should().BeFalse();

            //validation without regex
            _validator.Validate(new Person { PhoneNumber = "test_phone_number" }).IsValid.Should().BeFalse();
            _validator.Validate(new Person { PhoneNumber = "" }).IsValid.Should().BeFalse();
            _validator.Validate(new Person { PhoneNumber = "123" }).IsValid.Should().BeFalse();
            _validator.Validate(new Person { PhoneNumber = "[0-9]{1,14}^" }).IsValid.Should().BeTrue();

            //validation with regex
            _customerSettings.PhoneNumberValidationUseRegex = true;
            _validator.Validate(new Person { PhoneNumber = "test_phone_number" }).IsValid.Should().BeFalse();
            _validator.Validate(new Person { PhoneNumber = "123456789" }).IsValid.Should().BeTrue();
            _validator.Validate(new Person { PhoneNumber = "" }).IsValid.Should().BeFalse();
            _validator.Validate(new Person { PhoneNumber = "+123456789" }).IsValid.Should().BeFalse();
        }
    }
}
