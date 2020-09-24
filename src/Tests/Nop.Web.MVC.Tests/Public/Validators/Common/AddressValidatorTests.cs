using FluentValidation.TestHelper;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Tests;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Common;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Common
{
    [TestFixture]
    public class AddressValidatorTests : BaseNopTest
    {
        private ILocalizationService _localizationService;
        private IStateProvinceService _stateProvinceService;

        [SetUp]
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
            validator.ShouldHaveValidationErrorFor(x => x.Email, model);
            model.Email = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.Email, model);
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
            validator.ShouldHaveValidationErrorFor(x => x.Email, model);
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
            validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
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
            validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
            model.FirstName = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
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
            validator.ShouldNotHaveValidationErrorFor(x => x.FirstName, model);
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
            validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
            model.LastName = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
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
            validator.ShouldNotHaveValidationErrorFor(x => x.LastName, model);
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
            validator.ShouldHaveValidationErrorFor(x => x.Company, model);
            model.Company = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.Company, model);
            
            //not required
            validator = new AddressValidator(_localizationService, _stateProvinceService,
                new AddressSettings
                {
                    CompanyEnabled = true,
                    CompanyRequired = false
                }, new CustomerSettings());
            model.Company = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.Company, model);
            model.Company = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.Company, model);
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
            validator.ShouldNotHaveValidationErrorFor(x => x.Company, model);
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
            validator.ShouldHaveValidationErrorFor(x => x.Address1, model);
            model.Address1 = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.Address1, model);

            //not required
            validator = new AddressValidator(_localizationService, _stateProvinceService,
                new AddressSettings
                {
                    StreetAddressEnabled = true,
                    StreetAddressRequired = false
                }, new CustomerSettings());
            model.Address1 = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.Address1, model);
            model.Address1 = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.Address1, model);
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
            validator.ShouldNotHaveValidationErrorFor(x => x.Address1, model);
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
            validator.ShouldHaveValidationErrorFor(x => x.Address2, model);
            model.Address2 = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.Address2, model);

            //not required
            validator = new AddressValidator(_localizationService, _stateProvinceService,
                new AddressSettings
                {
                    StreetAddress2Enabled = true,
                    StreetAddress2Required = false
                }, new CustomerSettings());
            model.Address2 = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.Address2, model);
            model.Address2 = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.Address2, model);
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
            validator.ShouldNotHaveValidationErrorFor(x => x.Address2, model);
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
            validator.ShouldHaveValidationErrorFor(x => x.ZipPostalCode, model);
            model.ZipPostalCode = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.ZipPostalCode, model);
            
            //not required
            validator = new AddressValidator(_localizationService, _stateProvinceService,
                new AddressSettings
                {
                    ZipPostalCodeEnabled = true,
                    ZipPostalCodeRequired = false
                }, new CustomerSettings());
            model.ZipPostalCode = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode, model);
            model.ZipPostalCode = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode, model);
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
            validator.ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode, model);
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
            validator.ShouldHaveValidationErrorFor(x => x.City, model);
            model.City = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.City, model);
            
            //not required
            validator = new AddressValidator(_localizationService, _stateProvinceService,
                new AddressSettings
                {
                    CityEnabled = true,
                    CityRequired = false
                }, new CustomerSettings());
            model.City = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.City, model);
            model.City = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.City, model);
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
            validator.ShouldNotHaveValidationErrorFor(x => x.City, model);
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
            validator.ShouldHaveValidationErrorFor(x => x.PhoneNumber, model);
            model.PhoneNumber = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.PhoneNumber, model);

            //not required
            validator = new AddressValidator(_localizationService, _stateProvinceService,
                new AddressSettings
                {
                    PhoneEnabled = true,
                    PhoneRequired = false
                }, new CustomerSettings());
            model.PhoneNumber = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber, model);
            model.PhoneNumber = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber, model);
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
            validator.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber, model);
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
            validator.ShouldHaveValidationErrorFor(x => x.FaxNumber, model);
            model.FaxNumber = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.FaxNumber, model);
            
            //not required
            validator = new AddressValidator(_localizationService, _stateProvinceService,
                new AddressSettings
                {
                    FaxEnabled = true,
                    FaxRequired = false
                }, new CustomerSettings());
            model.FaxNumber = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.FaxNumber, model);
            model.FaxNumber = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.FaxNumber, model);
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
            validator.ShouldNotHaveValidationErrorFor(x => x.FaxNumber, model);
        }
    }
}
