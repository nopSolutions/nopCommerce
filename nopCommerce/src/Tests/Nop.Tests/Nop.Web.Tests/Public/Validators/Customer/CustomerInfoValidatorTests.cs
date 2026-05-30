using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer;

[TestFixture]
public class CustomerInfoValidatorTests : BaseNopTest
{
    private ILocalizationService _localizationService;
    private IStateProvinceService _stateProvinceService;
    private TaxSettings _taxSettings;

    [OneTimeSetUp]
    public void SetUp()
    {
        _localizationService = GetService<ILocalizationService>();
        _stateProvinceService = GetService<IStateProvinceService>();
        _taxSettings = GetService<TaxSettings>();
    }

    private CustomerInfoValidator GetDefaultValidator()
    {
        return new CustomerInfoValidator(_localizationService, _stateProvinceService, GetService<CustomerSettings>(), _taxSettings);
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
        var validator = GetDefaultValidator();

        var model = new CustomerInfoModel
        {
            Email = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        model.Email = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsWrongFormat()
    {
        var validator = GetDefaultValidator();

        var model = new CustomerInfoModel
        {
            Email = "adminexample.com"
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
    {
        var validator = GetDefaultValidator();

        var model = new CustomerInfoModel
        {
            Email = "admin@example.com"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void ShouldHaveErrorWhenFirstNameIsNullOrEmpty()
    {
        var customerSettings = new CustomerSettings
        {
            FirstNameEnabled = true,
            FirstNameRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        var model = new CustomerInfoModel
        {
            FirstName = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
        model.FirstName = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenFirstNameIsSpecified()
    {
        var customerSettings = new CustomerSettings
        {
            FirstNameEnabled = true,
            FirstNameRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        var model = new CustomerInfoModel
        {
            FirstName = "John"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    public void ShouldHaveErrorWhenLastNameIsNullOrEmpty()
    {
        var model = new CustomerInfoModel();
        //required
        var customerSettings = new CustomerSettings
        {
            LastNameEnabled = true,
            LastNameRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.LastName = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
        model.LastName = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);

        //not required
        customerSettings.LastNameRequired = false;
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.LastName = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
        model.LastName = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenLastNameIsSpecified()
    {
        var customerSettings = new CustomerSettings
        {
            LastNameEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        var model = new CustomerInfoModel
        {
            LastName = "Smith"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
    }

    [Test]
    public void ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new CustomerInfoModel();

        //required
        var customerSettings = new CustomerSettings
        {
            CompanyEnabled = true,
            CompanyRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Company = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);
        model.Company = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);

        //not required
        customerSettings.CompanyRequired = false;
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Company = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
        model.Company = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
    }

    [Test]
    public void ShouldNotHaveErrorWhenCompanyIsSpecified()
    {
        var customerSettings = new CustomerSettings
        {
            CompanyEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        var model = new CustomerInfoModel
        {
            Company = "Company"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
    }

    [Test]
    public void ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new CustomerInfoModel();

        //required
        var customerSettings = new CustomerSettings
        {
            StreetAddressEnabled = true,
            StreetAddressRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.StreetAddress = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress);
        model.StreetAddress = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress);

        //not required
        customerSettings.StreetAddressRequired = false;
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.StreetAddress = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
        model.StreetAddress = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
    }

    [Test]
    public void ShouldNotHaveErrorWhenStreetAddressIsSpecified()
    {
        var customerSettings = new CustomerSettings
        {
            StreetAddressEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        var model = new CustomerInfoModel
        {
            StreetAddress = "Street address"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
    }

    [Test]
    public void PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new CustomerInfoModel();

        //required
        var customerSettings = new CustomerSettings
        {
            StreetAddress2Enabled = true,
            StreetAddress2Required = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.StreetAddress2 = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress2);
        model.StreetAddress2 = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress2);

        //not required
        customerSettings.StreetAddress2Required = false;
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.StreetAddress2 = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
        model.StreetAddress2 = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
    }

    [Test]
    public void ShouldNotHaveErrorWhenStreetAddress2IsSpecified()
    {
        var customerSettings = new CustomerSettings
        {
            StreetAddress2Enabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        var model = new CustomerInfoModel
        {
            StreetAddress2 = "Street address 2"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
    }

    [Test]
    public void ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new CustomerInfoModel();

        //required
        var customerSettings = new CustomerSettings
        {
            ZipPostalCodeEnabled = true,
            ZipPostalCodeRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.ZipPostalCode = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);
        model.ZipPostalCode = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);

        //not required
        customerSettings.ZipPostalCodeRequired = false;
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.ZipPostalCode = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
        model.ZipPostalCode = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
    }

    [Test]
    public void ShouldNotHaveErrorWhenZipPostalCodeIsSpecified()
    {
        var customerSettings = new CustomerSettings
        {
            ZipPostalCodeEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        var model = new CustomerInfoModel
        {
            ZipPostalCode = "zip"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
    }

    [Test]
    public void ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new CustomerInfoModel();

        //required
        var customerSettings = new CustomerSettings
        {
            CityEnabled = true,
            CityRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.City = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);
        model.City = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);

        //not required
        customerSettings.CityRequired = false;
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.City = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
        model.City = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
    }

    [Test]
    public void ShouldNotHaveErrorWhenCityIsSpecified()
    {
        var customerSettings = new CustomerSettings
        {
            CityEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        var model = new CustomerInfoModel
        {
            City = "City"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
    }

    [Test]
    public void ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new CustomerInfoModel();

        //required
        var customerSettings = new CustomerSettings
        {
            PhoneEnabled = true,
            PhoneRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Phone = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Phone);
        model.Phone = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Phone);

        //not required
        customerSettings.PhoneRequired = false;
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Phone = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
        model.Phone = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Test]
    public void ShouldNotHaveErrorWhenPhoneIsSpecified()
    {
        var customerSettings = new CustomerSettings
        {
            PhoneEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        var model = new CustomerInfoModel
        {
            Phone = "Phone"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Test]
    public void ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new CustomerInfoModel();

        //required
        var customerSettings = new CustomerSettings
        {
            FaxEnabled = true,
            FaxRequired = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Fax = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Fax);
        model.Fax = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Fax);

        //not required
        customerSettings.FaxRequired = false;
        validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        model.Fax = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
        model.Fax = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
    }

    [Test]
    public void ShouldNotHaveErrorWhenFaxIsSpecified()
    {
        var customerSettings = new CustomerSettings
        {
            FaxEnabled = true
        };

        var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, customerSettings, _taxSettings);

        var model = new CustomerInfoModel
        {
            Fax = "Fax"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
    }
}