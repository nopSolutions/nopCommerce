using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Customer
{
    [TestFixture]
    public class CustomerInfoValidatorTests : BaseNopTest
    {
        private ILocalizationService _localizationService;
        private IStateProvinceService _stateProvinceService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _localizationService = GetService<ILocalizationService>();
            _stateProvinceService = GetService<IStateProvinceService>();
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
        {
            var validator = GetService<CustomerInfoValidator>();

            var model = new CustomerInfoModel
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
            var validator = GetService<CustomerInfoValidator>();

            var model = new CustomerInfoModel
            {
                Email = "adminexample.com"
            };
            validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var validator = GetService<CustomerInfoValidator>();

            var model = new CustomerInfoModel
            {
                Email = "admin@example.com"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void ShouldHaveErrorWhenFirstNameIsNullOrEmpty()
        {
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    FirstNameEnabled = true,
                    FirstNameRequired = true
                });

            var model = new CustomerInfoModel
            {
                FirstName = null
            };
            validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
            model.FirstName = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenFirstNameIsSpecified()
        {
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    FirstNameEnabled = true,
                    FirstNameRequired = true
                });

            var model = new CustomerInfoModel
            {
                FirstName = "John"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void ShouldHaveErrorWhenLastNameIsNullOrEmpty()
        {
            var model = new CustomerInfoModel();

            //required
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    LastNameEnabled = true,
                    LastNameRequired = true
                });
            model.LastName = null;
            validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
            model.LastName = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
            
            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    LastNameEnabled = true,
                    LastNameRequired = false
                });
            model.LastName = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.LastName, model);
            model.LastName = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.LastName, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenLastNameIsSpecified()
        {
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    LastNameEnabled = true
                });

            var model = new CustomerInfoModel
            {
                LastName = "Smith"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.LastName, model);
        }

        [Test]
        public void ShouldHaveErrorWhenCompanyIsNullOrEmptyBasedOnRequiredSetting()
        {
            var model = new CustomerInfoModel();

            //required
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    CompanyEnabled = true,
                    CompanyRequired = true
                });
            model.Company = null;
            validator.ShouldHaveValidationErrorFor(x => x.Company, model);
            model.Company = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.Company, model);
            
            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    CompanyEnabled = true,
                    CompanyRequired = false
                });
            model.Company = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.Company, model);
            model.Company = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.Company, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenCompanyIsSpecified()
        {
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, 
                new CustomerSettings
                {
                    CompanyEnabled = true
                });

            var model = new CustomerInfoModel
            {
                Company = "Company"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.Company, model);
        }

        [Test]
        public void ShouldHaveErrorWhenStreetAddressIsNullOrEmptyBasedOnRequiredSetting()
        {
            var model = new CustomerInfoModel();

            //required
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    StreetAddressEnabled = true,
                    StreetAddressRequired = true
                });
            model.StreetAddress = null;
            validator.ShouldHaveValidationErrorFor(x => x.StreetAddress, model);
            model.StreetAddress = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.StreetAddress, model);

            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    StreetAddressEnabled = true,
                    StreetAddressRequired = false
                });
            model.StreetAddress = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress, model);
            model.StreetAddress = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenStreetAddressIsSpecified()
        {
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    StreetAddressEnabled = true
                });

            var model = new CustomerInfoModel
            {
                StreetAddress = "Street address"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress, model);
        }

        [Test]
        public void PublicVoidShouldHaveErrorWhenStreetAddress2IsNullOrEmptyBasedOnRequiredSetting()
        {
            var model = new CustomerInfoModel();

            //required
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    StreetAddress2Enabled = true,
                    StreetAddress2Required = true
                });
            model.StreetAddress2 = null;
            validator.ShouldHaveValidationErrorFor(x => x.StreetAddress2, model);
            model.StreetAddress2 = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.StreetAddress2, model);

            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    StreetAddress2Enabled = true,
                    StreetAddress2Required = false
                });
            model.StreetAddress2 = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress2, model);
            model.StreetAddress2 = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress2, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenStreetAddress2IsSpecified()
        {
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, 
                new CustomerSettings
                {
                    StreetAddress2Enabled = true
                });

            var model = new CustomerInfoModel
            {
                StreetAddress2 = "Street address 2"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress2, model);
        }

        [Test]
        public void ShouldHaveErrorWhenZipPostalCodeIsNullOrEmptyBasedOnRequiredSetting()
        {
            var model = new CustomerInfoModel();

            //required
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    ZipPostalCodeEnabled = true,
                    ZipPostalCodeRequired = true
                });
            model.ZipPostalCode = null;
            validator.ShouldHaveValidationErrorFor(x => x.ZipPostalCode, model);
            model.ZipPostalCode = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.ZipPostalCode, model);
            
            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    ZipPostalCodeEnabled = true,
                    ZipPostalCodeRequired = false
                });
            model.ZipPostalCode = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode, model);
            model.ZipPostalCode = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenZipPostalCodeIsSpecified()
        {
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, 
                new CustomerSettings
                {
                    StreetAddress2Enabled = true
                });

            var model = new CustomerInfoModel
            {
                ZipPostalCode = "zip"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode, model);
        }

        [Test]
        public void ShouldHaveErrorWhenCityIsNullOrEmptyBasedOnRequiredSetting()
        {
            var model = new CustomerInfoModel();

            //required
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    CityEnabled = true,
                    CityRequired = true
                });
            model.City = null;
            validator.ShouldHaveValidationErrorFor(x => x.City, model);
            model.City = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.City, model);
            
            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    CityEnabled = true,
                    CityRequired = false
                });
            model.City = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.City, model);
            model.City = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.City, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenCityIsSpecified()
        {
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, 
                new CustomerSettings
                {
                    CityEnabled = true
                });

            var model = new CustomerInfoModel
            {
                City = "City"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.City, model);
        }

        [Test]
        public void ShouldHaveErrorWhenPhoneIsNullOrEmptyBasedOnRequiredSetting()
        {
            var model = new CustomerInfoModel();

            //required
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    PhoneEnabled = true,
                    PhoneRequired = true
                });
            model.Phone = null;
            validator.ShouldHaveValidationErrorFor(x => x.Phone, model);
            model.Phone = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.Phone, model);

            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    PhoneEnabled = true,
                    PhoneRequired = false
                });
            model.Phone = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.Phone, model);
            model.Phone = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.Phone, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenPhoneIsSpecified()
        {
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService, 
                new CustomerSettings
                {
                    PhoneEnabled = true
                });

            var model = new CustomerInfoModel
            {
                Phone = "Phone"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.Phone, model);
        }

        [Test]
        public void ShouldHaveErrorWhenFaxIsNullOrEmptyBasedOnRequiredSetting()
        {
            var model = new CustomerInfoModel();

            //required
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    FaxEnabled = true,
                    FaxRequired = true
                });
            model.Fax = null;
            validator.ShouldHaveValidationErrorFor(x => x.Fax, model);
            model.Fax = string.Empty;
            validator.ShouldHaveValidationErrorFor(x => x.Fax, model);
            
            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    FaxEnabled = true,
                    FaxRequired = false
                });
            model.Fax = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.Fax, model);
            model.Fax = string.Empty;
            validator.ShouldNotHaveValidationErrorFor(x => x.Fax, model);
        }

        [Test]
        public void ShouldNotHaveErrorWhenFaxIsSpecified()
        {
            var validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    FaxEnabled = true
                });

            var model = new CustomerInfoModel
            {
                Fax = "Fax"
            };
            validator.ShouldNotHaveValidationErrorFor(x => x.Fax, model);
        }
    }
}
