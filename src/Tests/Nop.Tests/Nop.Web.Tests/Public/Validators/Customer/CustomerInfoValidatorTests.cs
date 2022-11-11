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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
            model.Email = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsWrongFormat()
        {
            var validator = GetService<CustomerInfoValidator>();

            var model = new CustomerInfoModel
            {
                Email = "adminexample.com"
            };
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var validator = GetService<CustomerInfoValidator>();

            var model = new CustomerInfoModel
            {
                Email = "admin@example.com"
            };
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
            model.FirstName = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FirstName);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FirstName);
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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
            model.LastName = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LastName);
            
            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    LastNameEnabled = true,
                    LastNameRequired = false
                });
            model.LastName = null;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
            model.LastName = string.Empty;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.LastName);
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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);
            model.Company = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Company);
            
            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    CompanyEnabled = true,
                    CompanyRequired = false
                });
            model.Company = null;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
            model.Company = string.Empty;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Company);
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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress);
            model.StreetAddress = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress);

            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    StreetAddressEnabled = true,
                    StreetAddressRequired = false
                });
            model.StreetAddress = null;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
            model.StreetAddress = string.Empty;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress);
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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress2);
            model.StreetAddress2 = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.StreetAddress2);

            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    StreetAddress2Enabled = true,
                    StreetAddress2Required = false
                });
            model.StreetAddress2 = null;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
            model.StreetAddress2 = string.Empty;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.StreetAddress2);
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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);
            model.ZipPostalCode = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ZipPostalCode);
            
            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    ZipPostalCodeEnabled = true,
                    ZipPostalCodeRequired = false
                });
            model.ZipPostalCode = null;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
            model.ZipPostalCode = string.Empty;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode);
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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);
            model.City = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.City);
            
            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    CityEnabled = true,
                    CityRequired = false
                });
            model.City = null;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
            model.City = string.Empty;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.City);
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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Phone);
            model.Phone = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Phone);

            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    PhoneEnabled = true,
                    PhoneRequired = false
                });
            model.Phone = null;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
            model.Phone = string.Empty;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Phone);
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
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Fax);
            model.Fax = string.Empty;
            validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Fax);
            
            //not required
            validator = new CustomerInfoValidator(_localizationService, _stateProvinceService,
                new CustomerSettings
                {
                    FaxEnabled = true,
                    FaxRequired = false
                });
            model.Fax = null;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
            model.Fax = string.Empty;
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
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
            validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Fax);
        }
    }
}
