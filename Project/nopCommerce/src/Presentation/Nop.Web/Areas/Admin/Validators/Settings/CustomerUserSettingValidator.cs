using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Settings;

public partial class CustomerUserSettingValidator : BaseNopValidator<CustomerUserSettingsModel>
{
    public CustomerUserSettingValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.CustomerSettings.PasswordMinLength)
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.CustomerUser.PasswordMinLength.GreaterThanZero"));

        RuleFor(x => x.CustomerSettings.PasswordMaxLength)
            .GreaterThanOrEqualTo(x => x.CustomerSettings.PasswordMinLength)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.CustomerUser.PasswordMaxLength.GreaterThanOrEqualMinLength"));
    }
}
