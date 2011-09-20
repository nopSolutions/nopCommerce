using System.Globalization;
using System.Threading;
using Nop.Tests;
using NUnit.Framework;
using Nop.Web.Framework.Validators;

namespace Nop.Web.MVC.Tests.Validators
{
    [TestFixture]
    public class CreditCardValidatorTests
    {
        TestValidator validator;
        
        [SetUp]
        public void Setup()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            validator = new TestValidator {
				v => v.RuleFor(x => x.CreditCard).IsCreditCard()
			};
        }

        [Test]
        public void IsValidTests()
        {
            // Optional value is not valid
            validator.Validate(new Person { CreditCard = null }).IsValid.ShouldBeFalse();

            // Simplest valid value
            validator.Validate(new Person { CreditCard = "0000000000000000" }).IsValid.ShouldBeTrue();

            // Good checksum
            validator.Validate(new Person { CreditCard = "1234567890123452" }).IsValid.ShouldBeTrue();

            // Good checksum, with dashes
            validator.Validate(new Person { CreditCard = "1234-5678-9012-3452" }).IsValid.ShouldBeTrue();

            // Bad checksum
            validator.Validate(new Person { CreditCard = "0000000000000001" }).IsValid.ShouldBeFalse();

            validator.Validate(new Person { CreditCard = "0000000000000000" }).IsValid.ShouldBeTrue();
        }
    }
}
