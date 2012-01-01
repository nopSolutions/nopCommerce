using FluentValidation.TestHelper;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Common;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators.Common
{
    [TestFixture]
    public class AddressValidatorTests : BaseValidatorTests
    {
        private AddressValidator _validator;
        
        [SetUp]
        public new void Setup()
        {
            _validator = new AddressValidator(_localizationService);
        }
        
        [Test]
        public void Should_have_error_when_firstName_is_null_or_empty()
        {
            var model = new AddressModel();
            model.FirstName = null;
            _validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
            model.FirstName = "";
            _validator.ShouldHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void Should_not_have_error_when_firstName_is_specified()
        {
            var model = new AddressModel();
            model.FirstName = "John";
            _validator.ShouldNotHaveValidationErrorFor(x => x.FirstName, model);
        }

        [Test]
        public void Should_have_error_when_lastName_is_null_or_empty()
        {
            var model = new AddressModel();
            model.LastName = null;
            _validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
            model.LastName = "";
            _validator.ShouldHaveValidationErrorFor(x => x.LastName, model);
        }

        [Test]
        public void Should_not_have_error_when_lastName_is_specified()
        {
            var model = new AddressModel();
            model.LastName = "Smith";
            _validator.ShouldNotHaveValidationErrorFor(x => x.LastName, model);
        }

        [Test]
        public void Should_have_error_when_email_is_null_or_empty()
        {
            var model = new AddressModel();
            model.Email = null;
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
            model.Email = "";
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_have_error_when_email_is_wrong_format()
        {
            var model = new AddressModel();
            model.Email = "adminexample.com";
            _validator.ShouldHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_not_have_error_when_email_is_correct_format()
        {
            var model = new AddressModel();
            model.Email = "admin@example.com";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Email, model);
        }

        [Test]
        public void Should_not_have_error_when_company_is_not_specified()
        {
            var model = new AddressModel();
            model.Company = null;
            _validator.ShouldNotHaveValidationErrorFor(x => x.Company, model);
        }

        [Test]
        public void Should_have_error_when_country_is_null()
        {
            var model = new AddressModel();
            model.CountryId = null;
            _validator.ShouldHaveValidationErrorFor(x => x.CountryId, model);
        }

        [Test]
        public void Should_not_have_error_when_country_is_specifed()
        {
            var model = new AddressModel();
            model.CountryId = 1;
            _validator.ShouldNotHaveValidationErrorFor(x => x.CountryId, model);
        }

        [Test]
        public void Should_not_have_error_when_stateProvince_is_not_specified()
        {
            var model = new AddressModel();
            model.StateProvinceId = null;
            _validator.ShouldNotHaveValidationErrorFor(x => x.StateProvinceId, model);
        }

        [Test]
        public void Should_have_error_when_city_is_null_or_empty()
        {
            var model = new AddressModel();
            model.City = null;
            _validator.ShouldHaveValidationErrorFor(x => x.City, model);
            model.City = "";
            _validator.ShouldHaveValidationErrorFor(x => x.City, model);
        }

        [Test]
        public void Should_not_have_error_when_city_is_specified()
        {
            var model = new AddressModel();
            model.City = "New York";
            _validator.ShouldNotHaveValidationErrorFor(x => x.City, model);
        }

        [Test]
        public void Should_have_error_when_address1_is_null_or_empty()
        {
            var model = new AddressModel();
            model.Address1 = null;
            _validator.ShouldHaveValidationErrorFor(x => x.Address1, model);
            model.Address1 = "";
            _validator.ShouldHaveValidationErrorFor(x => x.Address1, model);
        }

        [Test]
        public void Should_not_have_error_when_address1_is_specified()
        {
            var model = new AddressModel();
            model.Address1 = "New York";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Address1, model);
        }

        [Test]
        public void Should_not_have_error_when_address2_is_not_specified()
        {
            var model = new AddressModel();
            model.Address2 = null;
            _validator.ShouldNotHaveValidationErrorFor(x => x.Address2, model);
        }

        [Test]
        public void Should_have_error_when_zipPostalCode_is_null_or_empty()
        {
            var model = new AddressModel();
            model.ZipPostalCode = null;
            _validator.ShouldHaveValidationErrorFor(x => x.ZipPostalCode, model);
            model.ZipPostalCode = "";
            _validator.ShouldHaveValidationErrorFor(x => x.ZipPostalCode, model);
        }

        [Test]
        public void Should_not_have_error_when_zipPostalCode_is_specified()
        {
            var model = new AddressModel();
            model.ZipPostalCode = "10001";
            _validator.ShouldNotHaveValidationErrorFor(x => x.ZipPostalCode, model);
        }

        [Test]
        public void Should_have_error_when_phoneNumber_is_null_or_empty()
        {
            var model = new AddressModel();
            model.PhoneNumber = null;
            _validator.ShouldHaveValidationErrorFor(x => x.PhoneNumber, model);
            model.PhoneNumber = "";
            _validator.ShouldHaveValidationErrorFor(x => x.PhoneNumber, model);
        }

        [Test]
        public void Should_not_have_error_when_phoneNumber_is_specified()
        {
            var model = new AddressModel();
            model.PhoneNumber = "123456789";
            _validator.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber, model);
        }

        [Test]
        public void Should_not_have_error_when_faxNumber_is_not_specified()
        {
            var model = new AddressModel();
            model.FaxNumber = null;
            _validator.ShouldNotHaveValidationErrorFor(x => x.FaxNumber, model);
        }
    }
}
