using AwesomeAssertions;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Validators;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

[TestFixture]
public class PhoneNumberValidatorTests
{
    private TestValidator _validator;
    private CustomerSettings _customerSettings;

    [OneTimeSetUp]
    public void Setup()
    {
        _customerSettings = new CustomerSettings
        {
            PhoneNumberValidationEnabled = true
        };

        _validator = new TestValidator { v => v.RuleFor(x => x.PhoneNumber).IsPhoneNumber(_customerSettings) };
    }

    [Test]
    public async Task IsValidTests()
    {
        //optional value is not valid
        _customerSettings.PhoneRequired = true;
        var result = await _validator.ValidateAsync(new Person { PhoneNumber = null });
        result.IsValid.Should().BeFalse();
        result = await _validator.ValidateAsync(new Person { PhoneNumber = string.Empty });
        result.IsValid.Should().BeFalse();
        result = await _validator.ValidateAsync(new Person { PhoneNumber = "test_phone_number" });
        result.IsValid.Should().BeFalse();
        result = await _validator.ValidateAsync(new Person { PhoneNumber = "123" });
        result.IsValid.Should().BeFalse();
        
        result = await _validator.ValidateAsync(new Person { PhoneNumber = "+14156667777" });
        result.IsValid.Should().BeTrue();
    }
}