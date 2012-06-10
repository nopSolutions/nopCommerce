using FluentValidation;
using Nop.Admin.Models.Plugins;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Plugins
{
    public class PluginValidator : AbstractValidator<PluginModel>
    {
        public PluginValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendlyName).NotNull().WithMessage(localizationService.GetResource("Admin.Configuration.Plugins.Fields.FriendlyName.Required"));
        }
    }
}