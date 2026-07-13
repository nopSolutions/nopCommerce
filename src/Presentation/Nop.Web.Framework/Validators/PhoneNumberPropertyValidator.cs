using FluentValidation;
using FluentValidation.Validators;
using Nop.Core.Domain.Customers;
using PhoneNumbers;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Phone number validator
/// </summary>
public partial class PhoneNumberPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
    protected readonly CustomerSettings _customerSettings;

    public override string Name => "PhoneNumberPropertyValidator";

    /// <summary>
    /// Ctor
    /// </summary>
    public PhoneNumberPropertyValidator(CustomerSettings customerSettings)
    {
        _customerSettings = customerSettings;
    }

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="context">Validation context</param>
    /// <returns>Result</returns>
    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
        return IsValid(value as string, _customerSettings);
    }

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="customerSettings">Customer settings</param>
    /// <returns>Result</returns>
    public static bool IsValid(string phoneNumber, CustomerSettings customerSettings)
    {
        if (!customerSettings.PhoneNumberValidationEnabled)
            return true;

        if (string.IsNullOrEmpty(phoneNumber))
            return !customerSettings.PhoneRequired;

        try
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var regionCode = phoneNumberUtil.GetRegionCodeForNumber(phoneNumberUtil.Parse(phoneNumber, null));
            var parsedPhoneNumber = phoneNumberUtil.Parse(phoneNumber, regionCode);

            return phoneNumberUtil.IsValidNumber(parsedPhoneNumber);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "Phone number is not valid";
}