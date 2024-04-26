using FluentValidation.TestHelper;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Common;

[TestFixture]
public class AddressValidatorTests : BaseNopTest
{
    private ILocalizationService _localizationService;
    private IStateProvinceService _stateProvinceService;

    [OneTimeSetUp]
    public void Setup()
    {
        _localizationService = GetService<ILocalizationService>();
        _stateProvinceService = GetService<IStateProvinceService>();
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
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
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
        {
            Email = "adminexample.com"
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
        {
            Email = "admin@example.com"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void ShouldHaveErrorWhenFirstnameIsNullOrEmpty()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
        {
            FirstName = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
        model.FirstName = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenFirstnameIsSpecified()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
        {
            FirstName = "John"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    public void ShouldHaveErrorWhenLastnameIsNullOrEmpty()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
        {
            LastName = null
        };
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
        model.LastName = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Test]
    public void ShouldNotHaveErrorWhenLastnameIsSpecified()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings(), new CustomerSettings());

        var model = new AddressModel
        {
            LastName = "Smith"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
    }

    [Test]
    public void ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new AddressModel();

        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CompanyEnabled = true,
                CompanyRequired = true
            }, new CustomerSettings());
        model.Company = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);
        model.Company = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CompanyEnabled = true,
                CompanyRequired = false
            }, new CustomerSettings());
        model.Company = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
        model.Company = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
    }

    [Test]
    public void ShouldNotHaveErrorWhenCompanyIsSpecified()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CompanyEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            Company = "Company"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
    }

    [Test]
    public void ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new AddressModel();

        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddressEnabled = true,
                StreetAddressRequired = true
            }, new CustomerSettings());
        model.Address1 = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Address1);
        model.Address1 = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Address1);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddressEnabled = true,
                StreetAddressRequired = false
            }, new CustomerSettings());
        model.Address1 = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address1);
        model.Address1 = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address1);
    }

    [Test]
    public void ShouldNotHaveErrorWhenStreetAddressIsSpecified()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddressEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            Address1 = "Street address"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address1);
    }

    [Test]
    public void ShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new AddressModel();

        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddress2Enabled = true,
                StreetAddress2Required = true
            }, new CustomerSettings());
        model.Address2 = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Address2);
        model.Address2 = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Address2);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddress2Enabled = true,
                StreetAddress2Required = false
            }, new CustomerSettings());
        model.Address2 = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address2);
        model.Address2 = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address2);
    }

    [Test]
    public void ShouldNotHaveErrorWhenStreetAddress2IsSpecified()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddress2Enabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            Address2 = "Street address 2"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Address2);
    }

    [Test]
    public void ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new AddressModel();

        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = true
            }, new CustomerSettings());
        model.ZipPostalCode = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);
        model.ZipPostalCode = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = false
            }, new CustomerSettings());
        model.ZipPostalCode = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
        model.ZipPostalCode = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
    }

    [Test]
    public void ShouldNotHaveErrorWhenZipPostalCodeIsSpecified()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                StreetAddress2Enabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            ZipPostalCode = "zip"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
    }

    [Test]
    public void ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new AddressModel();

        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CityEnabled = true,
                CityRequired = true
            }, new CustomerSettings());
        model.City = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);
        model.City = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CityEnabled = true,
                CityRequired = false
            }, new CustomerSettings());
        model.City = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
        model.City = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
    }

    [Test]
    public void ShouldNotHaveErrorWhenCityIsSpecified()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                CityEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            City = "City"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
    }

    [Test]
    public void ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new AddressModel();

        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                PhoneEnabled = true,
                PhoneRequired = true
            }, new CustomerSettings());
        model.PhoneNumber = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        model.PhoneNumber = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.PhoneNumber);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                PhoneEnabled = true,
                PhoneRequired = false
            }, new CustomerSettings());
        model.PhoneNumber = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
        model.PhoneNumber = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Test]
    public void ShouldNotHaveErrorWhenPhoneIsSpecified()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                PhoneEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            PhoneNumber = "Phone"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Test]
    public void ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()
    {
        var model = new AddressModel();

        //required
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                FaxEnabled = true,
                FaxRequired = true
            }, new CustomerSettings());
        model.FaxNumber = null;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FaxNumber);
        model.FaxNumber = string.Empty;
        validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FaxNumber);

        //not required
        validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                FaxEnabled = true,
                FaxRequired = false
            }, new CustomerSettings());
        model.FaxNumber = null;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FaxNumber);
        model.FaxNumber = string.Empty;
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FaxNumber);
    }

    [Test]
    public void ShouldNotHaveErrorWhenFaxIsSpecified()
    {
        var validator = new AddressValidator(_localizationService, _stateProvinceService,
            new AddressSettings
            {
                FaxEnabled = true
            }, new CustomerSettings());

        var model = new AddressModel
        {
            FaxNumber = "Fax"
        };
        validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FaxNumber);
    }
}