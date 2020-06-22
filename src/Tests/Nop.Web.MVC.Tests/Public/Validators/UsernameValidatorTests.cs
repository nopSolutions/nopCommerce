using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Validators;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators
{
    [TestFixture]
    public class UsernameValidatorTests
    {
        private TestValidator _validator;
        private CustomerSettings _customerSettings;
        
        [SetUp]
        public void Setup()
        {
            _customerSettings = new CustomerSettings
            {
                UsernameValidationRule = "^a.*1$",
                UsernameValidationEnabled = true,
                UsernameValidationUseRegex = false
            };

            _validator = new TestValidator { v => v.RuleFor(x => x.Username).IsUsername(_customerSettings) };
        }

        [Test]
        public void IsValidTests()
        {
            //optional value is not valid
            _validator.Validate(new Person { Username = null }).IsValid.Should().BeFalse();
            _validator.Validate(new Person { Username = string.Empty }).IsValid.Should().BeFalse();

            //validation without regex
            _validator.Validate(new Person { Username = "test_user" }).IsValid.Should().BeFalse();
            _validator.Validate(new Person { Username = "a*1^" }).IsValid.Should().BeTrue();

            //validation with regex
            _customerSettings.UsernameValidationUseRegex = true;
            _validator.Validate(new Person { Username = "test_user" }).IsValid.Should().BeFalse();
            _validator.Validate(new Person { Username = "a*1^" }).IsValid.Should().BeFalse();
            _validator.Validate(new Person { Username = "a1" }).IsValid.Should().BeTrue();
            _validator.Validate(new Person { Username = "a_test_user_name_1" }).IsValid.Should().BeTrue();
            _validator.Validate(new Person { Username = "b_test_user_name_1" }).IsValid.Should().BeFalse();
        }
    }
}
