using AwesomeAssertions;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Validators;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators;

[TestFixture]
public class PhoneNumberValidatorTests
{
    private static bool IsValid(string phone, CustomerSettings settings, string regionCode)
        => PhoneNumberPropertyValidator<Person, string>.IsValid(phone, settings, regionCode);

    [Test]
    public void ShouldBeValidWhenValidationDisabled()
    {
        var settings = new CustomerSettings { PhoneNumberValidationEnabled = false, PhoneRequired = true };

        IsValid("not-a-phone", settings, "US").Should().BeTrue();
        IsValid(null, settings, "US").Should().BeTrue();
    }

    [Test]
    public void EmptyPhoneShouldRespectPhoneRequired()
    {
        var settings = new CustomerSettings { PhoneNumberValidationEnabled = true, PhoneRequired = true };
        IsValid(null, settings, "US").Should().BeFalse();
        IsValid(string.Empty, settings, "US").Should().BeFalse();

        settings.PhoneRequired = false;
        IsValid(null, settings, "US").Should().BeTrue();
        IsValid(string.Empty, settings, "US").Should().BeTrue();
    }

    [Test]
    public void ValidLocalNumberWithRegionShouldBeValid()
    {
        var settings = new CustomerSettings { PhoneNumberValidationEnabled = true };
        IsValid("541-754-3010", settings, "US").Should().BeTrue();
    }

    [Test]
    public void InvalidNumberShouldBeInvalid()
    {
        var settings = new CustomerSettings { PhoneNumberValidationEnabled = true };
        IsValid("123", settings, "US").Should().BeFalse();
        IsValid("test_phone_number", settings, "US").Should().BeFalse();
    }

    [Test]
    public void InternationalNumberShouldValidateRegardlessOfRegion()
    {
        var settings = new CustomerSettings { PhoneNumberValidationEnabled = true };
        IsValid("+15417543010", settings, null).Should().BeTrue();
    }

    [Test]
    public void LocalNumberWithoutRegionShouldBeInvalid()
    {
        var settings = new CustomerSettings { PhoneNumberValidationEnabled = true };
        IsValid("5417543010", settings, null).Should().BeFalse();
    }
}
