using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Web.Framework.Validators;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators
{
    [TestFixture]
    public class CreditCardValidatorTests
    {
        private TestValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            _validator = new TestValidator { v => v.RuleFor(x => x.CreditCard).IsCreditCard() };
        }

        [Test]
        public async Task IsValidTests()
        {
            // Optional value is not valid
            var result = await _validator.ValidateAsync(new Person { CreditCard = null });
            result.IsValid.Should().BeFalse();

            // Simplest valid value
            result = await _validator.ValidateAsync(new Person { CreditCard = "0000000000000000" });
            result.IsValid.Should().BeTrue();

            // Good checksum
            result = await _validator.ValidateAsync(new Person { CreditCard = "1234567890123452" });
            result.IsValid.Should().BeTrue();

            // Good checksum, with dashes
            result = await _validator.ValidateAsync(new Person { CreditCard = "1234-5678-9012-3452" });
            result.IsValid.Should().BeTrue();

            // Good checksum, with spaces
            result = await _validator.ValidateAsync(new Person { CreditCard = "1234 5678 9012 3452" });
            result.IsValid.Should().BeTrue();

            // Bad checksum
            result = await _validator.ValidateAsync(new Person { CreditCard = "0000000000000001" });
            result.IsValid.Should().BeFalse();

            result = await _validator.ValidateAsync(new Person { CreditCard = "0000000000000000" });
            result.IsValid.Should().BeTrue();
        }
    }
}
