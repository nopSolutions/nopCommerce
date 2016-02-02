using FluentValidation.TestHelper;
using Nop.Admin.Models.Settings;
using Nop.Admin.Validators.Settings;
using Nop.Web.MVC.Tests.Public.Validators;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Admin.Validators.Settings
{
    [TestFixture]
    public class SettingValidatorTests : BaseValidatorTests
    {
        private SettingValidator _validator;

        [SetUp]
        public new void Setup()
        {
            _validator = new SettingValidator(_localizationService);
        }

        [Test]
        public void Should_have_error_when_name_contains_pageSizeOptions_and_value_has_duplicate_items()
        {
            var model = new SettingModel();
            model.Name = "catalogsettings.defaultcategorypagesizeoptions";
            model.Value = "1, 2, 3, 3";
            _validator.ShouldHaveValidationErrorFor(x => x.Value, model);
        }

        [Test]
        public void Should_not_have_error_when_name_contains_pageSizeOptions_and_value_has_not_duplicate_items()
        {
            var model = new SettingModel();
            model.Name = "catalogsettings.defaultcategorypagesizeoptions";
            model.Value = "1, 2, 3, 9";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Value, model);
        }

        [Test]
        public void Should_have_error_when_name_contains_pageSizeOptions_and_value_is_null_or_empty()
        {
            var model = new SettingModel();
            model.Name = "catalogsettings.defaultcategorypagesizeoptions";
            model.Value = null;
            _validator.ShouldHaveValidationErrorFor(x => x.Value, model);
            model.Value = "";
            _validator.ShouldHaveValidationErrorFor(x => x.Value, model);
        }

        [Test]
        public void Should_not_have_error_when_name_does_not_contain_pageSizeOptions_regardless_of_value()
        {
            var model = new SettingModel();
            model.Name = "test";
            model.Value = null;
            _validator.ShouldNotHaveValidationErrorFor(x => x.Value, model);
            model.Value = "";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Value, model);
            model.Value = "blahblah";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Value, model);
            model.Value = "1,2,3,4,4,4,4";
            _validator.ShouldNotHaveValidationErrorFor(x => x.Value, model);
        }
    }
}
