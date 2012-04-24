using System.Globalization;
using System.Threading;
using Nop.Tests;
using Nop.Web.Framework.Validators;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators
{
    [TestFixture]
    public class CreditCardValidatorTests
    {
        TestValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            _validator = new TestValidator {
				v => v.RuleFor(x => x.CreditCard).IsCreditCard()
			};
        }

        [Test]
        public void IsValidTests()
        {
            // Optional value is not valid
            _validator.Validate(new Person { CreditCard = null }).IsValid.ShouldBeFalse();

            // Simplest valid value
            _validator.Validate(new Person { CreditCard = "0000000000000000" }).IsValid.ShouldBeTrue();

            // Good checksum
            _validator.Validate(new Person { CreditCard = "1234567890123452" }).IsValid.ShouldBeTrue();

            // Good checksum, with dashes
            _validator.Validate(new Person { CreditCard = "1234-5678-9012-3452" }).IsValid.ShouldBeTrue();

            // Good checksum, with spaces
            _validator.Validate(new Person { CreditCard = "1234 5678 9012 3452" }).IsValid.ShouldBeTrue();

            // Bad checksum
            _validator.Validate(new Person { CreditCard = "0000000000000001" }).IsValid.ShouldBeFalse();

            _validator.Validate(new Person { CreditCard = "0000000000000000" }).IsValid.ShouldBeTrue();
        }
    }
}
