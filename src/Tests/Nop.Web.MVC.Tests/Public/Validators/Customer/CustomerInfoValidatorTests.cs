using FluentValidation.TestHelper;
using Nop.Core.Domain.Customers;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Customer;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Customer
{
    [TestFixture]
    public class CustomerInfoValidatorTests : BaseValidatorTests
    {
        [Test]
        public void Should_have_error_when_email_is_null_or_empty()
        {
            var customerSettings = new CustomerSettings();
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.Email = null;
            validator.ShouldHaveValidationErrorFor(x => x.Email, model);
            model.Email = "";
            validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }
        [Test]
        public void Should_have_error_when_email_is_wrong_format()
        {
            var customerSettings = new CustomerSettings();
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.Email = "adminexample.com";
            validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }
        [Test]
        public void Should_not_have_error_when_email_is_correct_format()
        {
            var customerSettings = new CustomerSettings();
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.Email = "admin@example.com";
            validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_have_error_when_firstName_is_null_or_empty()
        {
            var customerSettings = new CustomerSettings();
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.FirstName = null;
            validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
            model.FirstName = "";
            validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
        }
        [Test]
        public void Should_not_have_error_when_firstName_is_specified()
        {
            var customerSettings = new CustomerSettings();
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.FirstName = "John";
            validator.ShouldNotHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void Should_have_error_when_lastName_is_null_or_empty()
        {
            var customerSettings = new CustomerSettings();
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.LastName = null;
            validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
            model.LastName = "";
            validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
        }
        [Test]
        public void Should_not_have_error_when_lastName_is_specified()
        {
            var customerSettings = new CustomerSettings();
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.LastName = "Smith";
            validator.ShouldNotHaveValidationErrorFor(x => x.LastName, model);
        }

        [Test]
        public void Should_have_error_when_company_is_null_or_empty_based_on_required_setting()
        {
            var customerSettings = new CustomerSettings()
            {
                CompanyEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            //required
            customerSettings.CompanyRequired = true;
            model.Company = null;
            validator.ShouldHaveValidationErrorFor(x => x.Company, model);
            model.Company = "";
            validator.ShouldHaveValidationErrorFor(x => x.Company, model);
            //not required
            customerSettings.CompanyRequired = false;
            model.Company = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.Company, model);
            model.Company = "";
            validator.ShouldNotHaveValidationErrorFor(x => x.Company, model);
        }
        [Test]
        public void Should_not_have_error_when_company_is_specified()
        {
            var customerSettings = new CustomerSettings()
            {
                CompanyEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.Company = "Company";
            validator.ShouldNotHaveValidationErrorFor(x => x.Company, model);
        }

        [Test]
        public void Should_have_error_when_streetaddress_is_null_or_empty_based_on_required_setting()
        {
            var customerSettings = new CustomerSettings()
            {
                StreetAddressEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            //required
            customerSettings.StreetAddressRequired = true;
            model.StreetAddress = null;
            validator.ShouldHaveValidationErrorFor(x => x.StreetAddress, model);
            model.StreetAddress = "";
            validator.ShouldHaveValidationErrorFor(x => x.StreetAddress, model);
            //not required
            customerSettings.StreetAddressRequired = false;
            model.StreetAddress = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress, model);
            model.StreetAddress = "";
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress, model);
        }
        [Test]
        public void Should_not_have_error_when_streetaddress_is_specified()
        {
            var customerSettings = new CustomerSettings()
            {
                StreetAddressEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.StreetAddress = "Street address";
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress, model);
        }

        [Test]
        public void Should_have_error_when_streetaddress2_is_null_or_empty_based_on_required_setting()
        {
            var customerSettings = new CustomerSettings()
            {
                StreetAddress2Enabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            //required
            customerSettings.StreetAddress2Required = true;
            model.StreetAddress2 = null;
            validator.ShouldHaveValidationErrorFor(x => x.StreetAddress2, model);
            model.StreetAddress2 = "";
            validator.ShouldHaveValidationErrorFor(x => x.StreetAddress2, model);
            //not required
            customerSettings.StreetAddress2Required = false;
            model.StreetAddress2 = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress2, model);
            model.StreetAddress2 = "";
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress2, model);
        }
        [Test]
        public void Should_not_have_error_when_streetaddress2_is_specified()
        {
            var customerSettings = new CustomerSettings()
            {
                StreetAddress2Enabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.StreetAddress2 = "Street address 2";
            validator.ShouldNotHaveValidationErrorFor(x => x.StreetAddress2, model);
        }

        [Test]
        public void Should_have_error_when_zippostalcode_is_null_or_empty_based_on_required_setting()
        {
            var customerSettings = new CustomerSettings()
            {
                ZipPostalCodeEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            //required
            customerSettings.ZipPostalCodeRequired = true;
            model.ZipPostalCode = null;
            validator.ShouldHaveValidationErrorFor(x => x.ZipPostalCode, model);
            model.ZipPostalCode = "";
            validator.ShouldHaveValidationErrorFor(x => x.ZipPostalCode, model);
            //not required
            customerSettings.ZipPostalCodeRequired = false;
            model.ZipPostalCode = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode, model);
            model.ZipPostalCode = "";
            validator.ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode, model);
        }
        [Test]
        public void Should_not_have_error_when_zippostalcode_is_specified()
        {
            var customerSettings = new CustomerSettings()
            {
                StreetAddress2Enabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.ZipPostalCode = "zip";
            validator.ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode, model);
        }

        [Test]
        public void Should_have_error_when_city_is_null_or_empty_based_on_required_setting()
        {
            var customerSettings = new CustomerSettings()
            {
                CityEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            //required
            customerSettings.CityRequired = true;
            model.City = null;
            validator.ShouldHaveValidationErrorFor(x => x.City, model);
            model.City = "";
            validator.ShouldHaveValidationErrorFor(x => x.City, model);
            //not required
            customerSettings.CityRequired = false;
            model.City = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.City, model);
            model.City = "";
            validator.ShouldNotHaveValidationErrorFor(x => x.City, model);
        }
        [Test]
        public void Should_not_have_error_when_city_is_specified()
        {
            var customerSettings = new CustomerSettings()
            {
                CityEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.City = "City";
            validator.ShouldNotHaveValidationErrorFor(x => x.City, model);
        }

        [Test]
        public void Should_have_error_when_phone_is_null_or_empty_based_on_required_setting()
        {
            var customerSettings = new CustomerSettings()
            {
                PhoneEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            //required
            customerSettings.PhoneRequired = true;
            model.Phone = null;
            validator.ShouldHaveValidationErrorFor(x => x.Phone, model);
            model.Phone = "";
            validator.ShouldHaveValidationErrorFor(x => x.Phone, model);
            //not required
            customerSettings.PhoneRequired = false;
            model.Phone = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.Phone, model);
            model.Phone = "";
            validator.ShouldNotHaveValidationErrorFor(x => x.Phone, model);
        }
        [Test]
        public void Should_not_have_error_when_phone_is_specified()
        {
            var customerSettings = new CustomerSettings()
            {
                PhoneEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.Phone = "Phone";
            validator.ShouldNotHaveValidationErrorFor(x => x.Phone, model);
        }

        [Test]
        public void Should_have_error_when_fax_is_null_or_empty_based_on_required_setting()
        {
            var customerSettings = new CustomerSettings()
            {
                FaxEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            //required
            customerSettings.FaxRequired = true;
            model.Fax = null;
            validator.ShouldHaveValidationErrorFor(x => x.Fax, model);
            model.Fax = "";
            validator.ShouldHaveValidationErrorFor(x => x.Fax, model);
            //not required
            customerSettings.FaxRequired = false;
            model.Fax = null;
            validator.ShouldNotHaveValidationErrorFor(x => x.Fax, model);
            model.Fax = "";
            validator.ShouldNotHaveValidationErrorFor(x => x.Fax, model);
        }
        [Test]
        public void Should_not_have_error_when_fax_is_specified()
        {
            var customerSettings = new CustomerSettings()
            {
                FaxEnabled = true
            };
            var validator = new CustomerInfoValidator(_localizationService, customerSettings);

            var model = new CustomerInfoModel();
            model.Fax = "Fax";
            validator.ShouldNotHaveValidationErrorFor(x => x.Fax, model);
        }
    }
}
